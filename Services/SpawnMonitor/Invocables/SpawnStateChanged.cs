#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Coravel.Invocable;
using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Services.SpawnMonitor.Invocables
{
    public class SpawnStateChanged : IInvocable, IInvocableWithPayload<EsiIncursion>
    {
        private readonly IDiscordService _discord;
        private readonly IRedis _redis;
        private readonly DateTime createdAt;

        public EsiIncursion Payload { get; set; }


        public SpawnStateChanged(IDiscordService discord, IRedis redis)
        {
            _discord = discord;
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
            SolarSystem? featuredSystem;

            {
                List<SolarSystem> systems = new();
                foreach (int systemId in Payload.InfestedSolarSystems)
                {
                    systems.Add(await _redis.Get<SolarSystem>($"system:{systemId}"));
                }

                featuredSystem = systems.FirstOrDefault(c => c.SiteType == SiteType.Headquarters)
                    ?? systems.FirstOrDefault(c => c.Id == Payload.StagingSystemId);
            }

            EmbedBuilder embed = new()
            {
                Color = Utils.SecStatusColor(featuredSystem.SecurityStatus),
                Title = $"Incursion in {constellation.Name} is {Payload.State}.",
            };

            DateTime estDespawnAt = createdAt.AddDays(Payload.State == State.Mobilizing ? 3 : 1);

            embed.AddField("Estimated Despawn:", estDespawnAt.DiscordTimestamps());
            embed.AddField("Estimated Spawn Window", string.Format("{0} - {1}",
                    estDespawnAt.AddHours(12).DiscordTimestamps(false),
                    estDespawnAt.AddHours(36).DiscordTimestamps(false)
                )
            );

            await _discord.IncursionSpawn(embed.Build(), featuredSystem.Security);
        }
    }
}
#pragma warning restore CS8602
#pragma warning restore CS8618