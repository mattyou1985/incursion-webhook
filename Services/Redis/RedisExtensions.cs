namespace IncursionWebhook.Services.Redis
{
    public static class RedisExtensions
    {
        /// <summary>Adds a Redis Service to the <see cref="IServiceCollection"/></summary>
        public static void AddRedis(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRedis, Redis>();
        }
    }
}
