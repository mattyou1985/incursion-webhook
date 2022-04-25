#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Coravel.Invocable;
using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;
using System.Text;

namespace IncursionWebhook.Services.SpawnMonitor.Invocables
{
    public class SpawnDetected : IInvocable, IInvocableWithPayload<EsiIncursion>
    {
        private readonly IDiscordService _discord;
        private readonly IEveSwagger _esi;
        private readonly IRedis _redis;
        private readonly DateTime createdAt;

        public EsiIncursion Payload { get; set; }

        public SpawnDetected(IDiscordService discord, IEveSwagger esi, IRedis redis)
        {
            _discord = discord;
            _esi = esi;
            _redis = redis;

            // This is the time we noticed the spawn was done
            // we store the value now, rather than in Invoke() 
            // so that it is as close to true as possible,
            // and not influenced by the job queue size
            createdAt = DateTime.UtcNow;
        }

        public async Task Invoke()
        {
            Constellation constellation = await _redis.Get<Constellation>($"constellation:{Payload.ConstellationId}");
            Region region = await _redis.Get<Region>($"region:{constellation.RegionId}");

            // Build a map of infested systems
            Dictionary<SiteType, List<SolarSystem>> systems = new()
            {
                { SiteType.Headquarters, new() },
                { SiteType.Assaults, new() },
                { SiteType.Vanguards, new() },
                { SiteType.None, new() }
            };

            foreach (int systemId in Payload.InfestedSolarSystems)
            {
                SolarSystem system = await _redis.Get<SolarSystem>($"system:{systemId}");
                if (system.Id == Payload.StagingSystemId)
                {
                    systems.Add(SiteType.Staging, new List<SolarSystem>() { system });
                    continue;
                }

                systems[system.SiteType].Add(system);
            }

            SolarSystem featuredSystem = systems[SiteType.Headquarters].FirstOrDefault() ?? systems[SiteType.Staging].First();

            // Start building the Embed
            EmbedBuilder embed = new()
            {
                Title = $"New {featuredSystem.Security} Spawn!",
                Color = Utils.SecStatusColor(featuredSystem.SecurityStatus),
                Description = string.Format("In {0} ({1})",
                    Utils.MarkdownUrl(constellation.DotlanUrl(region.Name), constellation.Name),
                    Utils.MarkdownUrl(region.DotlanUrl, region.Name)
                ),
                Footer = new()
                {
                    Text = $"Est despawn between {createdAt.AddDays(4).DiscordTimestamps(false)} and {createdAt.AddDays(8).DiscordTimestamps(false)}."
                }
            };

            // List all of the systems by the site type
            foreach (SiteType t in Enum.GetValues(typeof(SiteType)))
            {
                // We do not want to display unknown types here
                if (t == SiteType.None) continue;

                StringBuilder sb = new();
                foreach (SolarSystem system in systems[t])
                {
                    sb.AppendLine(string.Format("{0} ({1} sec)",
                        Utils.MarkdownUrl(system.DotlanUrl(region.Name), system.Name),
                        system.SecurityStatus
                    ));
                }

                // Headquarters should be on their own line, everything else can be inline.
                embed.AddField($"{t}", sb.ToString().AsNullIfEmpty() ?? "\u200b", t != SiteType.Headquarters);
            }

            // Calculate distance to hubs
            List<string> remarks = new();

            SolarSystem? hub = await _esi.FindClosestHub(featuredSystem.Id);
            if (hub is null) remarks.Add("Spawn is Unreachable");
            else
            {
                List<SolarSystem> safest, shortest;
                safest = await _esi.GetRouteAsync(featuredSystem.Id, hub.Id);
                if (featuredSystem.Security == Security.Highsec && safest.Any(c => c.SecurityStatus < 0.5))
                {
                    remarks.Add("Island Spawn");
                }

                StringBuilder sb = new();
                sb.Append(string.Format("**{0} (Safest)** ",
                    Utils.MarkdownUrl($"https://eve-gatecheck.space/eve/#{hub.Name}:{featuredSystem.Name}:secure", $"{safest.Count} Jumps")));

                shortest = await _esi.GetRouteAsync(featuredSystem.Id, hub.Id, RouteFlag.shortest);
                if (!shortest.SequenceEqual(safest))
                {
                    sb.Append(string.Format("\n *{0} (Shortest)*",
                        Utils.MarkdownUrl($"https://eve-gatecheck.space/eve/#{hub.Name}:{featuredSystem.Name}:shortest", $"{shortest.Count} Jumps")));
                }

                embed.AddField($"Closest Hub: {hub.Name}", sb.ToString());
            }

            if (systems[SiteType.None].Any())
            {
                StringBuilder sb = new();
                systems[SiteType.None].ForEach(system => sb.Append($"{Utils.MarkdownUrl(system.DotlanUrl(region.Name), system.Name)}, "));
                remarks.Add($"\nUnknown: {sb.ToString().TrimEnd(',', ' ')}");
            }

            embed.AddField("Remarks:", string.Join(", ", remarks).TrimStart('\n').AsNullIfEmpty() ?? "n/a");

            await _discord.IncursionSpawn(embed.Build(), featuredSystem.Security, true);
        }
    }
}
#pragma warning restore CS8618