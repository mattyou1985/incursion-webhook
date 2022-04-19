using Discord;
using IncursionWebhook.Models;

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
        
        /// <summary>Send a ping to webhooks that monitor Incursion spawns</summary>
        Task IncursionSpawn(Embed embed);
    }
}
