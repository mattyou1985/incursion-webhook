using Newtonsoft.Json;

namespace IncursionWebhook.Services.Discord
{
    public class WebhookClient : IWebhookClient
    {
        /// <inheritdoc cref="IWebhookClient.TryCreate(Uri, out DiscordWebhook, out string?)"/>
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
