#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Coravel.Invocable;
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
        private readonly IWebhookClient _client;

        public EsiIncursion Payload { get; set; }

        public IncursionSpawnDown(IRedis redis, IWebhookClient webhookClient)
        {
            _client = webhookClient;
            _redis = redis;
        }

        public async Task Invoke()
        {
            Constellation constellation = await _redis.Get<Constellation>($"constellation:{Payload.ConstellationId}");
            await _client.SpawnDownAsync(constellation.Name ?? "Unknown");
        }
    }
}
#pragma warning restore CS8618