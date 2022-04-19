using IncursionWebhook.Models;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.EveSwagger
{
    /// <inheritdoc cref="IEveSwagger"/>
    public class EveSwagger : IEveSwagger
    {
        private readonly Uri ESI_URI = new("https://esi.evetech.net/");
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly IRedis _redis;

        public EveSwagger(ILogger<EveSwagger> logger, IRedis redis)
        {
            _logger = logger;
            _redis = redis;

            // Setup an HTTP Client
            _client = new();
            _client.BaseAddress = ESI_URI;
            _client.DefaultRequestHeaders.Add("ESI-Contact-Person", "Nyx Viliana");
        }

        /// <inheritdoc cref="IEveSwagger.GetRouteAsync(int, int, RouteFlag)"/>
        public async Task<List<SolarSystem>> GetRouteAsync(int originSystemId, int destinationSystemId, RouteFlag mode = RouteFlag.secure)
        {
            _logger.LogDebug("ESI GET: /latest/route/{0}/{1}", originSystemId, destinationSystemId);
            HttpResponseMessage res = await _client.GetAsync($"latest/route/{originSystemId}/{destinationSystemId}?flag={mode}");
            if (!res.IsSuccessStatusCode)
            {
                _logger.LogError($"[{res.StatusCode}] GET/latest/route/{originSystemId}/{destinationSystemId}: {res.ReasonPhrase}");
                return null;
            }

            List<SolarSystem> systems = new();
            foreach(int systemId in JsonConvert.DeserializeObject<List<int>>(await res.Content.ReadAsStringAsync()))
            {
                systems.Add(await _redis.Get<SolarSystem>($"system:{systemId}"));
            }

            return systems;
        }

        /// <inheritdoc cref="IEveSwagger.GetIncursionsAsync"/>
        public async Task<List<EsiIncursion>> GetIncursionsAsync()
        {
            _logger.LogDebug("ESI GET: /latest/incursions");
            HttpResponseMessage res = await _client.GetAsync("latest/incursions/");
            if (!res.IsSuccessStatusCode)
            {
                _logger.LogError($"[{res.StatusCode}] GET/latest/incursions: {res.ReasonPhrase}");
                return default;
            }

            return JsonConvert.DeserializeObject<List<EsiIncursion>>(await res.Content.ReadAsStringAsync());
        }

        /// <inheritdoc cref="IEveSwagger.FindClosestHub(int)"/>
        public async Task<SolarSystem> FindClosestHub(int originSystemId)
        {
            List<SolarSystem> closest = null;

            int[] hubs = new[] {
                30000142,  // Jita
                30002187,  // Amarr
                30002659,  // Dodixie
                30002510   // Rens
            };

            // Find the route to each trade hub,
            // and keep the route to the closest hub
            foreach (int systemId in hubs)
            {
                var systems = await GetRouteAsync(originSystemId, systemId);
                if (closest is null || systems.Count < closest.Count) closest = systems; 
            }

            // Return the last system in the shortest route.
            return closest?.Last();
        }
    }
}
