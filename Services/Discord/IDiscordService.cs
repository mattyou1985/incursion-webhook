using IncursionWebhook.Models;
using IncursionWebhook.Services.Redis;

namespace IncursionWebhook.Services.Discord
{
    public interface IDiscordService
    {
        /// <summary>Attempt to connect to a Discord Webhook and retrieve its settings </summary>
        /// <param name="webhookUrl">The Webhook URL provided by Discord</param>
        /// <param name="webhook"><em>if successful:</em> the webhook settings as reported by Discord</param>
        /// <param name="error"><em>if error:</em> the reason the webhook could not be found</param>
        /// <returns>A Boolean that indicates weather a webhook could be found</returns>
        bool TryCreate(Uri webhookUrl, out DiscordWebhook? webhook, out string? error);

        Task SpawnDetected(Region region, Constellation constellation, SolarSystem HQs, SolarSystem Staging, List<SolarSystem> Assaults, List<SolarSystem> VGs);

        /// <summary>Create the <em>Spawn Mobilizing</em> message</summary>
        /// <remarks>
        /// todo: Take in an incursion object and <br/>
        /// 1. Change {{constellation}} to Incursion.Constellation.Name <br/>
        /// 2. Change the colour of the embed dynamically based on sec status
        /// </remarks>
        Task SpawnMobilizing(string constellationName, SolarSystem system);

        /// <summary>Create the <em>Spawn Withdrawing</em> message</summary>
        /// <remarks>
        /// todo: Take in an incursion object and <br/>
        /// 1. Change {{constellation}} to Incursion.Constellation.Name <br/>
        /// 2. Change the colour of the embed dynamically based on sec status
        /// </remarks>
        Task SpawnWithdrawing(string constellationName, SolarSystem system);

        /// <summary>Create the <em>Spawn Down</em> message</summary>
        Task SpawnDownAsync(string constellationName);
    }
}
