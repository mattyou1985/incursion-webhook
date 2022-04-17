using IncursionWebhook.Services.EveSwagger.Models;

namespace IncursionWebhook.Services.EveSwagger
{
    public interface IEveSwagger
    {
        Task<int> DistanceBetweenAsync(int originSystemId, int destinationSystemId, RouteFlag mode = RouteFlag.Secure);

        /// <summary>Get the current Incursions from ESI</summary>
        Task<List<EsiIncursion>> GetIncursionsAsync();
    }
}
