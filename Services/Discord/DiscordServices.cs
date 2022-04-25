#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
using Discord;
using IncursionWebhook.Models;
using IncursionWebhook.Services.EveSwagger;
using IncursionWebhook.Services.Redis;
using IncursionWebhook.Services.SpawnMonitor.Models;
using Newtonsoft.Json;

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

        /// <inheritdoc cref="IDiscordService.IncursionSpawn(Embed, Security, bool?)"/>
        public async Task IncursionSpawn(Embed embed, Security securityType, bool mentionPingGroup = false)
        {
            List<SpawnWebhook> webhooks = await _redis.Get<List<SpawnWebhook>>("spawnWebhooks") ?? new();

            // Send webhook messages
            webhooks.ForEach(async webhook =>
            {
                string text = string.Empty;
                if(mentionPingGroup && webhook.PingGroup is not null)
                {
                    text = $"<@&{webhook.PingGroup}>";
                }

                // Do not send a message if the webhook has the securityType disabled
                object? x = webhook?.GetType()?.GetProperty(securityType.ToString())?.GetValue(webhook);
                if (bool.Parse(x.ToString())) 
                {
                    await webhook.SendMessageAsync(text, embeds: new[] { embed });
                }
            });
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
