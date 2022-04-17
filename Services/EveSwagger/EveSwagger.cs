using IncursionWebhook.Services.EveSwagger.Models;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.EveSwagger
{
    /// <inheritdoc cref="IEveSwagger"/>
    public class EveSwagger : IEveSwagger
    {
        private readonly Uri ESI_URI = new("https://esi.evetech.net/");
        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public EveSwagger(ILogger<EveSwagger> logger)
        {
            _logger = logger;

            // Setup an HTTP Client
            _client = new();
            _client.BaseAddress = ESI_URI;
            _client.DefaultRequestHeaders.Add("ESI-Contact-Person", "Nyx Viliana");
        }


        public Task<int> DistanceBetweenAsync(int originSystemId, int destinationSystemId, RouteFlag mode = RouteFlag.Secure)
        {
            throw new NotImplementedException();
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
    }
}
