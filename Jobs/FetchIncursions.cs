using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using IncursionWebhook.Services.EveSwagger;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Jobs
{
    public class FetchIncursions : IInvocable
    {
        private readonly IEveSwagger _esi;
        private readonly ILogger<FetchIncursions> _logger;
        private readonly IQueue _queue;
        private readonly IRedis _redis;
        
        public FetchIncursions(IEveSwagger esi, ILogger<FetchIncursions> logger, IQueue queue, IRedis redis)
        {
            _esi = esi;
            _logger = logger;
            _queue = queue;
            _redis = redis;
        }

        public async Task Invoke()
        {
            List<EsiIncursion> knownIncursions = await _redis.Get<List<EsiIncursion>>("incursions") ?? new();
            List<EsiIncursion> esiIncursions = await _esi.GetIncursionsAsync();

            // Foreach through the incursions reported by ESI
            foreach (EsiIncursion incursion in esiIncursions)
            {
                // Try to find the incursion in knownIncursions (saved list from REDIS)
                EsiIncursion? res = knownIncursions?.FirstOrDefault(inc => inc.ConstellationId == incursion.ConstellationId);

                // If we cannot find the incursion, that means it is new and we need to 
                // queue the NewIncursion invocable so that we can build the information required for the ping
                if (res is null)
                {
                    //todo: Queue NewIncursionJob
                    continue;
                }

                // The incursion state has changed, we need to queue an invocable to send a ping
                if(res.State != incursion.State)
                {
                    _queue.QueueInvocableWithPayload<IncursionStateChange, EsiIncursion>(incursion);
                    continue;
                }
            }

            // Look for incursions that have ended
            foreach(EsiIncursion incursion in knownIncursions)
            {
                if(!esiIncursions.Any(c => c.ConstellationId == incursion.ConstellationId))
                {
                    // todo: Consider passing {incursion} so we can state which spawn is down
                    _logger.LogInformation($"Incursion in {{constellation name}} down");
                    _queue.QueueInvocable<IncursionSpawnDown>();
                }
            }

            await _redis.Set("incursions", esiIncursions);
        }
    }
}
