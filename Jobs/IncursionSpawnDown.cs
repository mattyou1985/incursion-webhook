#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Coravel.Invocable;
using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Jobs
{
    /// <summary>Reports that an incursion has despawned.</summary>
    public class IncursionSpawnDown : IInvocable, IInvocableWithPayload<EsiIncursion>
    {
        private readonly IRedis _redis;
        private readonly IDiscordService _client;
        private readonly DateTime createdAt;

        public EsiIncursion Payload { get; set; }

        public IncursionSpawnDown(IRedis redis, IDiscordService webhookClient)
        {
            _client = webhookClient;
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

            EmbedBuilder embed = new()
            {
                Color = Color.DarkGrey,
                Title = $"Incursion in {constellation.Name ?? "Unknown"} is Over."
            };

            // The spawn window opens 12 hours from now and lasts 24 hours.
            embed.AddField("Spawn Window Opens:", createdAt.AddHours(12).DiscordTimestamps(), true);
            embed.AddField("Spawn Window Closes:", createdAt.AddHours(36).DiscordTimestamps(), true);

            await _client.IncursionSpawn(embed.Build());
        }
    }
}
#pragma warning restore CS8618