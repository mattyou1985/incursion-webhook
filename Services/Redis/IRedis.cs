namespace IncursionWebhook.Services.Redis
{
    public interface IRedis
    {
        /// <summary>Retrieves an item from the database</summary>
        /// <typeparam name="T">The <em>type</em> expected to be returned</typeparam>
        /// <param name="key">The <em>key</em> associated with the target item</param>
        Task<T> Get<T>(string key);

        /// <summary>Stores/Updates an item in the database</summary>
        /// <param name="key">The <em>key</em> associated with the target item</param>
        /// <param name="data">The data that will be stored</param>
        Task Set<T>(string key, T data);

        /// <summary>Deletes an item from the database</summary>
        /// <param name="key">The <em>key</em> associated with the target item</param>
        Task Delete(string key);
    }
}
