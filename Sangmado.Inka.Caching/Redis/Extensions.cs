using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using Sangmado.Inka.Serialization;

namespace Sangmado.Inka.Caching.Redis
{
    internal static class Extensions
    {
        public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return source.Select(item => (RedisValue)JsonConvert.SerializeObject(item)).ToArray();
        }

        public static T To<T>(this RedisValue redisValue)
        {
            var @value = (string)redisValue;
            if (string.IsNullOrWhiteSpace(@value))
                return default(T);

            return JsonConvert.DeserializeObject<T>(redisValue);
        }

        public static string ToRedisValue(this object source)
        {
            return JsonConvert.SerializeObject(source);
        }
    }
}
