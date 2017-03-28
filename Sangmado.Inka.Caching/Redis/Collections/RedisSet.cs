using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace Sangmado.Inka.Caching.Redis.Collections
{
    public class RedisSet<T> : ISet<T>, ICollection<T>, IReadOnlyCollection<T>
    {
        private const string RedisKeyTemplate = "Set:{0}";

        private readonly IDatabase _db;
        private readonly string _redisKey;

        public RedisSet(IDatabase database, string name)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            _db = database;
            _redisKey = string.Format(RedisKeyTemplate, name);
        }

        public bool Add(T item)
        {
            return _db.SetAdd(_redisKey, item.ToRedisValue());
        }

        public long Add(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            return _db.SetAdd(_redisKey, items.ToRedisValues());
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SetCombineAndStore(SetOperation.Difference, other);
        }

        public void ExceptWith(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SetCombineAndStore(SetOperation.Difference, other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SetCombineAndStore(SetOperation.Intersect, other);
        }

        public void IntersectWith(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SetCombineAndStore(SetOperation.Intersect, other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return Count < other.Count() && IsSubsetOf(other);
        }

        public bool IsProperSubsetOf(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return Count < other.Count && IsSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return Count > other.Count() && IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return Count > other.Count && IsSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return !this.Except(other).Any();
        }

        public bool IsSubsetOf(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return !SetCombine(SetOperation.Difference, other).Any();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return !other.Except(this).Any();
        }

        public bool IsSupersetOf(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return !other.SetCombine(SetOperation.Difference, this).Any();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            foreach (var item in other)
            {
                if (Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Overlaps(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return SetCombine(SetOperation.Intersect, other).Any();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return IsSubsetOf(other) && IsSupersetOf(other);
        }

        public bool SetEquals(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return IsSubsetOf(other) && IsSupersetOf(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            var otherSet = new RedisSet<T>(_db, Guid.NewGuid().ToString());
            try
            {
                otherSet.Add(other);
                SymmetricExceptWith(otherSet);
            }
            finally
            {
                otherSet.Clear();
            }
        }

        public void SymmetricExceptWith(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            var intersectedSet = new RedisSet<T>(_db, Guid.NewGuid().ToString());
            try
            {
                SetCombineAndStore(SetOperation.Intersect, intersectedSet, this, other);
                SetCombineAndStore(SetOperation.Union, other);
                SetCombineAndStore(SetOperation.Difference, intersectedSet);
            }
            finally
            {
                intersectedSet.Clear();
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SetCombineAndStore(SetOperation.Union, other);
        }

        public void UnionWith(RedisSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SetCombineAndStore(SetOperation.Union, other);
        }

        public void Clear()
        {
            _db.KeyDelete(_redisKey);
        }

        public bool Contains(T item)
        {
            return _db.SetContains(_redisKey, item.ToRedisValue());
        }

        public int Count
        {
            get
            {
                long count = _db.SetLength(_redisKey);
                if (count > int.MaxValue)
                {
                    throw new OverflowException("Count exceeds maximum value of integer.");
                }
                return (int)count;
            }
        }

        public bool Remove(T item)
        {
            return _db.SetRemove(_redisKey, item.ToRedisValue());
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<T>.CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (array.Length - index < this.Count)
            {
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
            }

            foreach (var item in this)
            {
                array[index++] = item;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _db
                    .SetScan(_redisKey)
                    .Select(redisValue => redisValue.To<T>())
                    .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void SetCombineAndStore(SetOperation operation, IEnumerable<T> other)
        {
            var redisTempSet = new RedisSet<T>(_db, Guid.NewGuid().ToString());
            try
            {
                redisTempSet.Add(other);
                SetCombineAndStore(operation, redisTempSet);
            }
            finally
            {
                redisTempSet.Clear();
            }
        }

        private void SetCombineAndStore(SetOperation operation, RedisSet<T> other)
        {
            SetCombineAndStore(operation, this, this, other);
        }

        private void SetCombineAndStore(SetOperation operation, RedisSet<T> destination, RedisSet<T> first, RedisSet<T> second)
        {
            _db.SetCombineAndStore(operation, destination._redisKey, first._redisKey, second._redisKey);
        }

        private RedisValue[] SetCombine(SetOperation operation, RedisSet<T> other)
        {
            return _db.SetCombine(operation, _redisKey, other._redisKey);
        }
    }
}
