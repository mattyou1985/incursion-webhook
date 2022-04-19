using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.EveSwagger;
using IncursionWebhook.Services.EveSwagger.Models;
using IncursionWebhook.Services.Redis;
using Newtonsoft.Json;
using System.Text;

namespace IncursionWebhook.Services.Discord
{
    public class DiscordServices : IDiscordService
    {
        private readonly IEveSwagger _esi;
        private readonly IRedis _redis;

        public DiscordServices(IEveSwagger esi, IRedis redis)
        {
            _esi = esi;
            _redis = redis;
        }

        /// <inheritdoc cref="IDiscordService.IncursionSpawn(Embed)"/>
        public async Task IncursionSpawn(Embed embed)
        {
            List<DiscordWebhook> webhooks = await _redis.Get<List<DiscordWebhook>>("discord-webhooks");
            //todo: remove non incursion spawn hooks

            // Send webhook messages
            webhooks.ForEach(async webhook => await webhook.SendMessageAsync(null, embeds: new[] { embed }));
        }

        /// <inheritdoc cref="IDiscordService.TryCreate(Uri, out DiscordWebhook, out string?)"/>
        public bool TryCreate(Uri webhookUrl, out DiscordWebhook? webhook, out string? error)
        {
            error = null;
            webhook = null;

            using HttpClient client = new();
            HttpResponseMessage res = client.GetAsync(webhookUrl).Result;
            if (!res.IsSuccessStatusCode)
            {
                error = res.ReasonPhrase;
                return false;
            }

            try
            {
                webhook = JsonConvert.DeserializeObject<DiscordWebhook>(res.Content.ReadAsStringAsync().Result);
                webhook.WebhookUrl = webhookUrl.ToString();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
