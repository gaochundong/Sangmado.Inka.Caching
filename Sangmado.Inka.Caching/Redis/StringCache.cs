using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Sangmado.Inka.Caching.Redis
{
    public class StringCache : ICacheClient, ICacheClientAsync
    {
        #region Ctors

        private IDatabase _db;

        public StringCache(IDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            _db = database;
        }

        #endregion

        #region ICacheClient Members

        public bool ContainsKey(string key)
        {
            return _db.KeyExists(key);
        }

        public T Get<T>(string key)
        {
            // Retrieves the specified item from the cache.
            return _db.StringGet(key).To<T>();
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any())
                return null;

            var dict = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var entity = Get<T>(key);
                dict.Add(key, entity);
            }

            return dict;
        }

        public bool Set<T>(string key, T value)
        {
            // Sets an item into the cache at the cache key specified regardless if it already exists or not.
            return _db.StringSet(key, value.ToRedisValue());
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            bool result = _db.StringSet(key, value.ToRedisValue());
            if (result)
                result = _db.KeyExpire(key, expiresAt);
            return result;
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            bool result = _db.StringSet(key, value.ToRedisValue());
            if (result)
                result = _db.KeyExpire(key, expiresIn);
            return result;
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            if (values == null || !values.Any())
                return;

            foreach (var item in values)
            {
                Set<T>(item.Key, item.Value);
            }
        }

        public bool Add<T>(string key, T value)
        {
            // Adds a new item into the cache at the specified cache key only if the cache is empty.
            if (!ContainsKey(key))
                return Set<T>(key, value);
            return true;
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            if (!ContainsKey(key))
                return Set<T>(key, value, expiresAt);
            return true;
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            if (!ContainsKey(key))
                return Set<T>(key, value, expiresIn);
            return true;
        }

        public bool Remove(string key)
        {
            // Removes the specified item from the cache.
            return _db.KeyDelete(key);
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any())
                return;

            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public bool Replace<T>(string key, T value)
        {
            // Replaces the item at the cache key specified only if an items exists at the location already. 
            if (ContainsKey(key))
                return Set<T>(key, value);
            return true;
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            if (ContainsKey(key))
                return Set<T>(key, value, expiresAt);
            return true;
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            if (ContainsKey(key))
                return Set<T>(key, value, expiresIn);
            return true;
        }

        public long Increment(string key, long amount)
        {
            // Increments the value of the specified key by the given amount. 
            // The operation is atomic and happens on the server.
            // A non existent value at key starts at 0.
            return _db.StringIncrement(key, amount);
        }

        public long Decrement(string key, long amount)
        {
            return _db.StringDecrement(key, amount);
        }

        #endregion

        #region ICacheClientAsync Members

        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            // Retrieves the specified item from the cache.
            var task = await _db.StringGetAsync(key)
                .ContinueWith(async (t) =>
                {
                    var item = await t;
                    return item.To<T>();
                });
            return await task;
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any())
                return null;

            var dict = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var entity = await GetAsync<T>(key);
                dict.Add(key, entity);
            }

            return dict;
        }

        public async Task<bool> SetAsync<T>(string key, T value)
        {
            // Sets an item into the cache at the cache key specified regardless if it already exists or not.
            return await _db.StringSetAsync(key, value.ToRedisValue());
        }

        public async Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt)
        {
            bool result = await _db.StringSetAsync(key, value.ToRedisValue());
            if (result)
                result = await _db.KeyExpireAsync(key, expiresAt);
            return result;
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            bool result = await _db.StringSetAsync(key, value.ToRedisValue());
            if (result)
                result = await _db.KeyExpireAsync(key, expiresIn);
            return result;
        }

        public async Task SetAllAsync<T>(IDictionary<string, T> values)
        {
            if (values == null || !values.Any())
                return;

            foreach (var item in values)
            {
                await SetAsync<T>(item.Key, item.Value);
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value)
        {
            // Adds a new item into the cache at the specified cache key only if the cache is empty.
            if (!await ContainsKeyAsync(key))
                return await SetAsync<T>(key, value);
            return true;
        }

        public async Task<bool> AddAsync<T>(string key, T value, DateTime expiresAt)
        {
            if (!await ContainsKeyAsync(key))
                return await SetAsync<T>(key, value, expiresAt);
            return true;
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            if (!await ContainsKeyAsync(key))
                return await SetAsync<T>(key, value, expiresIn);
            return true;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            // Removes the specified item from the cache.
            return await _db.KeyDeleteAsync(key);
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any())
                return;

            foreach (var key in keys)
            {
                await RemoveAsync(key);
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value)
        {
            // Replaces the item at the cache key specified only if an items exists at the location already. 
            if (await ContainsKeyAsync(key))
                return await SetAsync<T>(key, value);
            return true;
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value, DateTime expiresAt)
        {
            if (await ContainsKeyAsync(key))
                return await SetAsync<T>(key, value, expiresAt);
            return true;
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            if (await ContainsKeyAsync(key))
                return await SetAsync<T>(key, value, expiresIn);
            return true;
        }

        public async Task<long> IncrementAsync(string key, long amount)
        {
            // Increments the value of the specified key by the given amount. 
            // The operation is atomic and happens on the server.
            // A non existent value at key starts at 0.
            return await _db.StringIncrementAsync(key, amount);
        }

        public async Task<long> DecrementAsync(string key, long amount)
        {
            return await _db.StringDecrementAsync(key, amount);
        }

        #endregion
    }
}
