using Coravel.Invocable;
using IncursionWebhook.Services.Discord;

namespace IncursionWebhook.Jobs
{
    public class IncursionSpawnDown : IInvocable
    {
        private readonly IWebhookClient _client;
        
        public IncursionSpawnDown(IWebhookClient webhookClient)
        {
            _client = webhookClient;
        }

        public async Task Invoke()
        {
            await _client.SpawnDownAsync();
        }
    }
}
