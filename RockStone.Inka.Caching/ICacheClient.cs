using System;
using System.Collections.Generic;

namespace RockStone.Inka.Caching
{
    public interface ICacheClient
    {
        bool ContainsKey(string key);

        // Retrieves the specified item from the cache.
        T Get<T>(string key);
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);

        // Sets an item into the cache at the cache key specified regardless if it already exists or not.
        bool Set<T>(string key, T value);
        bool Set<T>(string key, T value, DateTime expiresAt);
        bool Set<T>(string key, T value, TimeSpan expiresIn);
        void SetAll<T>(IDictionary<string, T> values);

        // Adds a new item into the cache at the specified cache key only if the cache is empty.
        bool Add<T>(string key, T value);
        bool Add<T>(string key, T value, DateTime expiresAt);
        bool Add<T>(string key, T value, TimeSpan expiresIn);

        // Removes the specified item from the cache.
        bool Remove(string key);
        void RemoveAll(IEnumerable<string> keys);

        // Replaces the item at the cache key specified only if an items exists at the location already. 
        bool Replace<T>(string key, T value);
        bool Replace<T>(string key, T value, DateTime expiresAt);
        bool Replace<T>(string key, T value, TimeSpan expiresIn);

        // Increments the value of the specified key by the given amount. 
        // The operation is atomic and happens on the server.
        // A non existent value at key starts at 0.
        long Increment(string key, long amount);
        long Decrement(string key, long amount);
    }

    public interface ICacheClient<T>
    {
        bool ContainsKey(string key);

        // Retrieves the specified item from the cache.
        T Get(string key);
        IDictionary<string, T> GetAll(IEnumerable<string> keys);

        // Sets an item into the cache at the cache key specified regardless if it already exists or not.
        bool Set(string key, T value);
        bool Set(string key, T value, DateTime expiresAt);
        bool Set(string key, T value, TimeSpan expiresIn);
        void SetAll(IDictionary<string, T> values);

        // Adds a new item into the cache at the specified cache key only if the cache is empty.
        bool Add(string key, T value);
        bool Add(string key, T value, DateTime expiresAt);
        bool Add(string key, T value, TimeSpan expiresIn);

        // Removes the specified item from the cache.
        bool Remove(string key);
        void RemoveAll(IEnumerable<string> keys);

        // Replaces the item at the cache key specified only if an items exists at the location already. 
        bool Replace(string key, T value);
        bool Replace(string key, T value, DateTime expiresAt);
        bool Replace(string key, T value, TimeSpan expiresIn);

        // Increments the value of the specified key by the given amount. 
        // The operation is atomic and happens on the server.
        // A non existent value at key starts at 0.
        long Increment(string key, long amount);
        long Decrement(string key, long amount);
    }
}
