#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Coravel.Invocable;
using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Services.SpawnMonitor.Invocables
{
    public class SpawnEnded : IInvocable, IInvocableWithPayload<EsiIncursion>
    {
        private readonly IDiscordService _discord;
        private readonly IRedis _redis;
        private readonly DateTime createdAt;

        public EsiIncursion Payload { get; set; }


        public SpawnEnded(IDiscordService discord, IRedis redis)
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
            SolarSystem stagingSystem = await _redis.Get<SolarSystem>($"system:{Payload.StagingSystemId}");

            EmbedBuilder embed = new()
            {
                Color = Color.DarkGrey,
                Title = $"Incursion in {constellation.Name ?? "Unknown"} is Over."
            };

            // The spawn window opens 12 hours from now and lasts 24 hours.
            embed.AddField("Spawn Window Opens:", createdAt.AddHours(12).DiscordTimestamps(), true);
            embed.AddField("Spawn Window Closes:", createdAt.AddHours(36).DiscordTimestamps(), true);

            await _discord.IncursionSpawn(embed.Build(), stagingSystem.Security);
        }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.