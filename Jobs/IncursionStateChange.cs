#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Coravel.Invocable;
using IncursionWebhook.Models;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Jobs
{
    public class IncursionStateChange : IInvocable, IInvocableWithPayload<EsiIncursion>
    {
        private readonly IDiscordService _client;
        private readonly IRedis _redis;

        public EsiIncursion Payload { get; set; }

        public IncursionStateChange(IRedis redis, IDiscordService client)
        {
            _redis = redis;
            _client = client;
        }

        public async Task Invoke()
        {
            List<SolarSystem> systems = new();
            foreach (int systemId in Payload.InfestedSolarSystems) systems.Add(await _redis.Get<SolarSystem>($"system:{systemId}"));

            Constellation constellation = await _redis.Get<Constellation>($"constellation:{Payload.ConstellationId}");
            SolarSystem? Hqs = systems.FirstOrDefault(c => c.SiteType == SiteType.Headquarters);
            SolarSystem Staging = systems.FirstOrDefault(c => c.Id == Payload.StagingSystemId);
            
            
            double secStatus = 0;

            switch (Payload.State)
            {
                case State.Mobilizing:
                    await _client.SpawnMobilizing(constellation.Name ?? "Unknown", Hqs ?? Staging ?? new());
                    //await _client.SpawnMobilizing(Payload);
                    break;

                case State.Withdrawing:
                    await _client.SpawnWithdrawing(constellation.Name ?? "Unknown", Hqs ?? Staging ?? new());
                    //await _client.SpawnWithdrawing(Payload);
                    break;
            }
        }
    }
}
#pragma warning restore CS8618