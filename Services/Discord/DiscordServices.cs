using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.Redis;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.Discord
{
    public class DiscordServices : IDiscordService
    {
        private readonly IRedis _redis;

        public DiscordServices(IRedis redis)
        {
            _redis = redis;
        }

        /// <inheritdoc cref="IDiscordService.SpawnDetected(Region, Constellation, SolarSystem, SolarSystem, List{SolarSystem}, List{SolarSystem})"/>
        public async Task SpawnDetected(Region region, Constellation constellation, SolarSystem HQs, SolarSystem Staging, List<SolarSystem> Assaults, List<SolarSystem> VGs)
        {
            EmbedBuilder embed = new()
            {
                Title = $"New {(HQs ?? Staging).Security} Spawn!",
                Color = Utils.SecStatusColor((HQs ?? Staging).SecurityStatus),
                Description = string.Format("In {0}, {1}", 
                    Utils.MarkdownUrl(Utils.DotlanUniverseUrl(region.Name, constellation.Name), constellation.Name),
                    Utils.MarkdownUrl(Utils.DotlanUniverseUrl(region.Name), region.Name)
                )
            };

            // List system(s) HQ, Assaults and VGs
            if (HQs is not null)
            {
                embed.AddField("Headquarters",
                    string.Format("{0} ({1}sec)", Utils.MarkdownUrl(Utils.DotlanUniverseUrl(region.Name, HQs.Name), HQs.Name), HQs.SecurityStatus), true);
            }

            if (Assaults is not null && Assaults.Count != 0)
            {
                embed.AddField("Assaults",
                    string.Join("\n", Assaults.Select(s =>
                        Utils.MarkdownUrl(Utils.DotlanUniverseUrl(region.Name, s.Name), s.Name))
                    ),
                    true
                );
            }

            if (VGs is not null && VGs.Count != 0)
            {
                embed.AddField("Vanguards",
                    string.Join("\n", VGs.Select(s =>
                        Utils.MarkdownUrl(Utils.DotlanUniverseUrl(region.Name, s.Name), s.Name))
                    ),
                    true
                );
            }


            //// Closest Hub
            //embed.AddField($"Closest Hub: {{system name}}", string.Format("  • {0} (Safest)\n  • {1} (Shortest)",
            //    Utils.MarkdownUrl("https://example.com", $"{{int}} Jumps"),
            //    Utils.MarkdownUrl("https://example.com", $"{{int}} Jumps")
            //));

            // Remarks - this could include: "No stations in HQ", "Island", "Unknown Systems <csv list>"
            embed.AddField("Remarks:", "n/a");

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IDiscordService.SpawnMobilizing(string, SolarSystem)"/>
        public async Task SpawnMobilizing(string constellationName, SolarSystem system)
        {
            DateTime now = DateTime.Now;

            EmbedBuilder embed = new()
            {
                Color = Utils.SecStatusColor(system.SecurityStatus),
                Title = $"Incursion in {constellationName} is Mobilizing.",
            };

            embed.AddField("Estimated Despawn:", now.AddDays(3).DiscordTimestamps());
            embed.AddField("Estimated Spawn Window", string.Format("{0} - {1}",
                    now.AddDays(3).AddHours(12).DiscordTimestamps(false),
                    now.AddDays(3).AddHours(36).DiscordTimestamps(false)
                )
            );

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IDiscordService.SpawnWithdrawing(string, SolarSystem)"/>
        public async Task SpawnWithdrawing(string constellationName, SolarSystem system)
        {
            DateTime now = DateTime.Now;

            EmbedBuilder embed = new()
            {
                Color = Utils.SecStatusColor(system.SecurityStatus),
                Title = $"Incursion in {constellationName} is Withdrawing.",
            };

            embed.AddField("Estimated Despawn:", now.AddDays(1).DiscordTimestamps());

            embed.AddField("Estimated Spawn Window", string.Format("{0} - {1}",
                    now.AddDays(1).AddHours(12).DiscordTimestamps(false),
                    now.AddDays(1).AddHours(36).DiscordTimestamps(false)
                )
            );

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IDiscordService.SpawnDownAsync(string)"/>
        public async Task SpawnDownAsync(string constellationName)
        {
            DateTime now = DateTime.UtcNow;

            EmbedBuilder embed = new()
            {
                Color = Color.DarkGrey,
                Title = $"Incursion in {constellationName} is Over."
            };

            embed.AddField("Spawn Window Opens:", now.AddHours(12).DiscordTimestamps(), true);
            embed.AddField("Spawn Window Closes:", now.AddHours(36).DiscordTimestamps(), true);

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IDiscordService.TryCreate(Uri, out DiscordWebhook, out string?)"/>
        public bool TryCreate(Uri webhookUrl, out DiscordWebhook? webhook, out string? error)
        {
            error = null;
            webhook = null;

            using HttpClient client = new();
            HttpResponseMessage res = client.GetAsync(webhookUrl).Result;
            if (!res.IsSuccessStatusCode)
            {
                error = res.ReasonPhrase;
                return false;
            }

            try
            {
                webhook = JsonConvert.DeserializeObject<DiscordWebhook>(res.Content.ReadAsStringAsync().Result);
                webhook.WebhookUrl = webhookUrl.ToString();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
