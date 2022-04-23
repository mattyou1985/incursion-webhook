using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using IncursionWebhook.Services.EveSwagger;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Services.SpawnMonitor.Invocables
{
    public class FetchIncursions : IInvocable
    {
        private readonly IEveSwagger _esi;
        private readonly IQueue _queue;
        private readonly IRedis _redis;
        
        public FetchIncursions(IEveSwagger esi, IQueue queue, IRedis redis)
        {
            _esi = esi;
            _queue = queue;
            _redis = redis;
        }

        public async Task Invoke()
        {
            List<EsiIncursion> knownIncursions = await _redis.Get<List<EsiIncursion>>("incursions") ?? new();
            List<EsiIncursion> esiIncursions = await _esi.GetIncursionsAsync() ?? new();

            // Foreach through the incursions reported by ESI
            foreach (EsiIncursion incursion in esiIncursions)
            {
                // Try to find the incursion in knownIncursions (saved list from REDIS)
                EsiIncursion? res = knownIncursions?.FirstOrDefault(inc => inc.ConstellationId == incursion.ConstellationId);

                // If we cannot find the incursion, that means it is new and we need to 
                // queue the NewIncursion invocable so that we can build the information required for the ping
                if (res is null)
                {
                    _queue.QueueInvocableWithPayload<SpawnDetected, EsiIncursion>(incursion);
                    continue;
                }

                // The incursion state has changed, we need to queue an invocable to send a ping
                if(res.State != incursion.State)
                {
                    _queue.QueueInvocableWithPayload<SpawnStateChanged, EsiIncursion>(incursion);
                    continue;
                }
            }

            // Look for incursions that have ended
            foreach(EsiIncursion incursion in knownIncursions)
            {
                if(!esiIncursions.Any(c => c.ConstellationId == incursion.ConstellationId))
                {
                    _queue.QueueInvocableWithPayload<SpawnEnded, EsiIncursion>(incursion);
                }
            }

            await _redis.Set("incursions", esiIncursions);
        }
    }
}
