using Coravel.Invocable;
using IncursionWebhook.Services.Discord;
using IncursionWebhook.Services.EveSwagger.Models;

namespace IncursionWebhook.Jobs
{
    public class IncursionStateChange : IInvocable, IInvocableWithPayload<EsiIncursion>
    {
        private readonly IWebhookClient _client;

        public EsiIncursion Payload { get; set; }

        public IncursionStateChange(IWebhookClient client) => _client = client;        

        public async Task Invoke()
        {
            switch (Payload.State)
            {
                case State.Mobilizing:
                    await _client.SpawnMobilizing(Payload);
                    break;

                case State.Withdrawing:
                    await _client.SpawnWithdrawing(Payload);
                    break;
            }
        }
    }
}
