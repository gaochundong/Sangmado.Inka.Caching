using System;
using System.Collections.Generic;
using StackExchange.Redis;
using RockStone.Inka.Extensions;

namespace RockStone.Inka.Caching.Redis.Collections
{
    public class RedisCacheFactory : ICacheFactory
    {
        private IDatabase _db;

        public RedisCacheFactory(IDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            _db = database;
        }

        public ISet<T> CreateSetCache<T>(string name)
        {
            return new RedisSet<T>(_db, name);
        }

        public IList<T> CreateListCache<T>(string name)
        {
            return new RedisList<T>(_db, name);
        }

        public IQueue<T> CreateQueueCache<T>(string name)
        {
            return new RedisQueue<T>(_db, name);
        }

        public IStack<T> CreateStackCache<T>(string name)
        {
            return new RedisStack<T>(_db, name);
        }

        public IDictionary<K, V> CreateDictionaryCache<K, V>(string name)
        {
            return new RedisDictionary<K, V>(_db, name);
        }
    }
}
