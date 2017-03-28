using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sangmado.Inka.Caching
{
    public interface ICacheClientAsync
    {
        Task<bool> ContainsKeyAsync(string key);

        // Retrieves the specified item from the cache.
        Task<T> GetAsync<T>(string key);
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);

        // Sets an item into the cache at the cache key specified regardless if it already exists or not.
        Task<bool> SetAsync<T>(string key, T value);
        Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt);
        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn);
        Task SetAllAsync<T>(IDictionary<string, T> values);

        // Adds a new item into the cache at the specified cache key only if the cache is empty.
        Task<bool> AddAsync<T>(string key, T value);
        Task<bool> AddAsync<T>(string key, T value, DateTime expiresAt);
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn);

        // Removes the specified item from the cache.
        Task<bool> RemoveAsync(string key);
        Task RemoveAllAsync(IEnumerable<string> keys);

        // Replaces the item at the cache key specified only if an items exists at the location already. 
        Task<bool> ReplaceAsync<T>(string key, T value);
        Task<bool> ReplaceAsync<T>(string key, T value, DateTime expiresAt);
        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn);

        // Increments the value of the specified key by the given amount. 
        // The operation is atomic and happens on the server.
        // A non existent value at key starts at 0.
        Task<long> IncrementAsync(string key, long amount);
        Task<long> DecrementAsync(string key, long amount);
    }

    public interface ICacheClientAsync<T>
    {
        Task<bool> ContainsKeyAsync(string key);

        // Retrieves the specified item from the cache.
        Task<T> GetAsync(string key);
        Task<IDictionary<string, T>> GetAllAsync(IEnumerable<string> keys);

        // Sets an item into the cache at the cache key specified regardless if it already exists or not.
        Task<bool> SetAsync(string key, T value);
        Task<bool> SetAsync(string key, T value, DateTime expiresAt);
        Task<bool> SetAsync(string key, T value, TimeSpan expiresIn);
        Task SetAllAsync(IDictionary<string, T> values);

        // Adds a new item into the cache at the specified cache key only if the cache is empty.
        Task<bool> AddAsync(string key, T value);
        Task<bool> AddAsync(string key, T value, DateTime expiresAt);
        Task<bool> AddAsync(string key, T value, TimeSpan expiresIn);

        // Removes the specified item from the cache.
        Task<bool> RemoveAsync(string key);
        Task RemoveAllAsync(IEnumerable<string> keys);

        // Replaces the item at the cache key specified only if an items exists at the location already. 
        Task<bool> ReplaceAsync(string key, T value);
        Task<bool> ReplaceAsync(string key, T value, DateTime expiresAt);
        Task<bool> ReplaceAsync(string key, T value, TimeSpan expiresIn);

        // Increments the value of the specified key by the given amount. 
        // The operation is atomic and happens on the server.
        // A non existent value at key starts at 0.
        Task<long> IncrementAsync(string key, long amount);
        Task<long> DecrementAsync(string key, long amount);
    }
}
