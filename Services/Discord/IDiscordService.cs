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
        /// <param name="embed">The embed that is to be sent to the webhooks</param>
        /// <param name="securityType">The type of security (High, Low or Null) affected by the incursion</param>
        /// <param name="mentionPingGroup"><em>boolean:</em> Indicates weather the designated ping group should be mentioned</param>
        Task IncursionSpawn(Embed embed, Security securityType, bool mentionPingGroup = false);
    }
}
