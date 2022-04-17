using Discord;
using Newtonsoft.Json;

namespace IncursionWebhook.Services.Discord
{
    public class WebhookClient : IWebhookClient
    {
        private readonly List<DiscordWebhook> _webhooks = new();

        public WebhookClient()
        {
            // We need to get a list of all webhooks and store it in the property above
        }

        /// <inheritdoc cref="IWebhookClient.SpawnMobilizing"/>
        public async Task SpawnMobilizing() // take in an Incursion
        {
            DateTime now = DateTime.Now;

            EmbedBuilder embed = new()
            {
                Color = Color.DarkGrey,// make this dynamic based on sec status
                Title = $"{{constellation}} is Mobilizing."
            };

            embed.AddField("Estimated Despawn:", now.AddDays(2).DiscordTimestamps());
            embed.AddField("Estimated Spawn Window", 
                string.Format("{0} - {1}",
                    now.AddDays(2).AddHours(12).DiscordTimestamps(false),
                    now.AddDays(2).AddHours(36).DiscordTimestamps(false)
                )
            );

            foreach (DiscordWebhook? webhook in _webhooks)
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

        /// <inheritdoc cref="IWebhookClient.SpawnDownAsync"/>
        public async Task SpawnDownAsync()
        {
            DateTime now = DateTime.UtcNow;

            EmbedBuilder embed = new()
            {
                Color = Color.DarkGrey,
                Title = "Spawn Down."
            };

            embed.AddField("Spawn Window Opens:", now.AddHours(12).DiscordTimestamps(), true);
            embed.AddField("Spawn Window Closes:", now.AddHours(36).DiscordTimestamps(), true);

            foreach (DiscordWebhook? webhook in _webhooks)
            {
                await webhook.SendMessageAsync(null, embeds: new[] { embed.Build() });
            }
        }

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
