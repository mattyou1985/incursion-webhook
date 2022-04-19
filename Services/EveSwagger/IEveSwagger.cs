using IncursionWebhook.Models;
using IncursionWebhook.Services.EveSwagger.Models;

namespace IncursionWebhook.Services.EveSwagger
{
    public interface IEveSwagger
    {
        /// <summary>Find the closest trade hub</summary>
        /// <param name="originSystemId">systemId of the starting system</param>
        /// <remarks><em>options:</em> Jita, Amarr, Rens, Dodixie</remarks>
        Task<SolarSystem> FindClosestHub(int originSystemId);

        /// <summary>Find the route between two systems.</summary>
        /// <param name="originSystemId">systemId of the starting system</param>
        /// <param name="destinationSystemId">systemId of the end system</param>
        /// <param name="mode">The route security preference</param>
        /// <returns>List of Systems in the route</returns>
        /// <remarks>GET: /latest/route/{origin}/{destination}</remarks>
        Task<List<SolarSystem>> GetRouteAsync(int originSystemId, int destinationSystemId, RouteFlag mode = RouteFlag.secure);

        /// <summary>Get the current Incursions from ESI</summary>
        /// <remarks>GET: /latest/incursions</remarks>
        Task<List<EsiIncursion>> GetIncursionsAsync();
    }
}
