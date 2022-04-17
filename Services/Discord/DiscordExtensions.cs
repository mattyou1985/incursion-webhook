namespace IncursionWebhook.Services.Discord
{
    public static class DiscordExtensions
    {
        /// <summary>Adds a DiscordWebhook to the <see cref="IServiceCollection"/></summary>
        public static void AddDiscordWebhook(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IWebhookClient, WebhookClient>();
        }
    }
}
