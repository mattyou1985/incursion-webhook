#pragma warning disable CS8603 // Possible null reference return.
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IncursionWebhook.Services.Redis
{
    public class Redis : IRedis
    {
        private ConnectionMultiplexer _client;

        // Create a new Redis connection
        public Redis()
        {
            string redisServer = Environment.GetEnvironmentVariable("REDIS_SERVER") ?? "localhost:6379";
            _client = ConnectionMultiplexer.Connect(redisServer);
        }

        // Safely complete data transactions and close our connection 
        // when the Garbage Collector starts cleaning up this class.
        ~Redis()
        {
            if (_client is not null && _client.IsConnected)
            {
                _client.Close();
                _client = null;
            }
        }

        /// <inheritdoc cref="IRedis.Delete(string)"/>
        public async Task Delete(string key)
        {
            IDatabase db = _client.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        /// <inheritdoc cref="IRedis.Get{T}(string)"/>
        public async Task<T> Get<T>(string key)
        {
            IDatabase db = _client.GetDatabase();
            var res = await db.StringGetAsync(key);

            if (res.IsNull) return default(T);
            else return JsonConvert.DeserializeObject<T>(res);
        }

        /// <inheritdoc cref="IRedis.Set{T}(string, T)"/>
        public async Task Set<T>(string key, T data)
        {
            IDatabase db = _client.GetDatabase();
            await db.StringSetAsync(key, JsonConvert.SerializeObject(data));
        }

    }
}
#pragma warning restore CS8603