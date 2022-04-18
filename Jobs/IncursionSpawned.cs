using Coravel.Invocable;
using IncursionWebhook.Models;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Jobs
{
    public class IncursionSpawned : IInvocable, IInvocableWithPayload<EsiIncursion>
    {
        private readonly IRedis _redis;
        private readonly IWebhookClient _client;

        public EsiIncursion Payload { get; set; }

        public IncursionSpawned(IRedis redis, IWebhookClient webhookClient)
        {
            _client = webhookClient;
            _redis = redis;
        }

        public async Task Invoke()
        {
            Constellation constellation = await _redis.Get<Constellation>($"constellation:{Payload.ConstellationId}");
            Region region = await _redis.Get<Region>($"region:{constellation.RegionId}");
            List<SolarSystem> solarSystems = new();

            foreach(int systemId in Payload.InfestedSolarSystems)
            {
                solarSystems.Add(await _redis.Get<SolarSystem>($"system:{systemId}"));
            }


            await _client.SpawnDetected(
                region,
                constellation,
                // Headquarters
                solarSystems.FirstOrDefault(c => c.SiteType == SiteType.Headquarters),

                // Staging
                solarSystems.FirstOrDefault(c => c.Id == Payload.StagingSystemId),

                // Assaults
                solarSystems.Where(c => c.SiteType == SiteType.Assaults).ToList(),
                // Vanguards
                solarSystems.Where(c => c.SiteType == SiteType.Vanguards).ToList()
            );
        }
    }
}
