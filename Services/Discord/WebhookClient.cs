using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.Discord
{
    public class WebhookClient : IWebhookClient
    {
        private readonly IRedis _redis;

        public WebhookClient(IRedis redis)
        {
            _redis = redis;
        }

        /// <inheritdoc cref="IWebhookClient.SpawnDetected"/>
        public async Task SpawnDetected() // take in an Incursion
        {
            EmbedBuilder embed = new()
            {
                Title = "New {{sector}} Spawn!",
                Color = Utils.SecStatusColor(-0.2), // make this dynamic based on sec status
                Description = string.Format("{0} < {1}",
                    Utils.MarkdownUrl(Utils.DotlanUniverseUrl("Sinq Laison", "Wyllequet"), "Wyllequet"),
                    Utils.MarkdownUrl(Utils.DotlanUniverseUrl("Sinq Laison"), "Sinq Laison")
                )
            };

            // List system(s) HQ, Assaults and VGs
            embed.AddField("Headquarters", string.Format("{0} ({1}AU, {2:0f}sec)",
                Utils.MarkdownUrl(Utils.DotlanUniverseUrl("The Forge", "Jita"), "Jita"), 91, 0.5), true
            );
            embed.AddField("Assaults", Utils.MarkdownUrl(Utils.DotlanUniverseUrl("Heimatar", "Rens"), "Rens"), true);
            embed.AddField("Vanguards", Utils.MarkdownUrl(Utils.DotlanUniverseUrl("Heimatar", "Rens"), "Rens") + "\n" +
                Utils.MarkdownUrl(Utils.DotlanUniverseUrl("Heimatar", "Rens"), "Rens") + "\n" +
                Utils.MarkdownUrl(Utils.DotlanUniverseUrl("Heimatar", "Rens"), "Rens"), true
            );

            // Closest Hub
            embed.AddField($"Closest Hub: {{system name}}", string.Format("  • {0} (Safest)\n  • {1} (Shortest)",
                Utils.MarkdownUrl("https://example.com", $"{{int}} Jumps"),
                Utils.MarkdownUrl("https://example.com", $"{{int}} Jumps")
            ));

            // Remarks - this could include
            // • "No stations in HQ"
            // • "Island Spawn"
            // • "Unknown Systems: <csv list>
            embed.AddField("Remarks:", "n/a");

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IWebhookClient.SpawnMobilizing"/>
        public async Task SpawnMobilizing(EsiIncursion incursion)
        {
            DateTime now = DateTime.Now;
            Constellation constellation = await _redis.Get<Constellation>($"constellation:{incursion.ConstellationId}");

            EmbedBuilder embed = new()
            {
                Color = Utils.SecStatusColor(-0.2),// make this dynamic based on sec status
                Title = $"Incursion in {constellation.Name ?? "Unknown"} is Mobilizing.",
            };

            embed.AddField("Estimated Despawn:", now.AddDays(3).DiscordTimestamps());
            embed.AddField("Estimated Spawn Window", 
                string.Format("{0} - {1}",
                    now.AddDays(3).AddHours(12).DiscordTimestamps(false),
                    now.AddDays(3).AddHours(36).DiscordTimestamps(false)
                )
            );

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IWebhookClient.SpawnWithdrawing"/>
        public async Task SpawnWithdrawing(EsiIncursion incursion)
        {
            DateTime now = DateTime.Now;
            Constellation constellation = await _redis.Get<Constellation>($"constellation:{incursion.ConstellationId}");

            EmbedBuilder embed = new()
            {
                Color = Utils.SecStatusColor(-0.2),// make this dynamic based on sec status
                Title = $"Incursion in {constellation.Name ?? "Unknown"} is Withdrawing."
            };

            embed.AddField("Estimated Despawn:", now.AddDays(2).DiscordTimestamps());
            embed.AddField("Estimated Spawn Window",
                string.Format("{0} - {1}",
                    now.AddDays(1).AddHours(12).DiscordTimestamps(false),
                    now.AddDays(1).AddHours(36).DiscordTimestamps(false)
                )
            );

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IWebhookClient.SpawnDownAsync"/>
        public async Task SpawnDownAsync()
        {
            DateTime now = DateTime.UtcNow;

            EmbedBuilder embed = new()
            {
                Color = Color.DarkGrey,
                Title = "Spawn Down."
            };

            embed.AddField("Spawn Window Opens:", now.AddHours(12).DiscordTimestamps(), true);
            embed.AddField("Spawn Window Closes:", now.AddHours(36).DiscordTimestamps(), true);

            foreach (DiscordWebhook? webhook in await _redis.Get<List<DiscordWebhook>>("discord-webhooks"))
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IWebhookClient.TryCreate(Uri, out DiscordWebhook, out string?)"/>
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
