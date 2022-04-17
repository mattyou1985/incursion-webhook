namespace IncursionWebhook.Services.EveSwagger
{
    public static class EveSwaggerExtensions
    {
        /// <summary>Adds a DiscordWebhook Service to the <see cref="IServiceCollection"/></summary>
        public static void AddEveSwagger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEveSwagger, EveSwagger>();
        }
    }
}
