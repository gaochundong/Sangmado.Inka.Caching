using System;
using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;

namespace RockStone.Inka.Caching.Redis.Collections
{
    public class RedisList<T> : IList<T>, ICollection<T>, IReadOnlyCollection<T>
    {
        private const string RedisKeyTemplate = "List:{0}";

        private static Exception IndexOutOfRangeException = new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");

        private readonly IDatabase _db;
        private readonly string _redisKey;

        public RedisList(IDatabase database, string name)
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

        public int IndexOf(T item)
        {
            int index = 0;
            foreach (var member in this)
            {
                if (EqualityComparer<T>.Default.Equals(member, item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            try
            {
                _db.ListSetByIndex(_redisKey, index, item.ToRedisValue());
            }
            catch (RedisServerException redisServerException)
            {
                if (IsIndexOutOfRangeExcepiton(redisServerException))
                {
                    throw IndexOutOfRangeException;
                }
                throw;
            }
        }

        public void RemoveAt(int index)
        {
            string deleteFlag = Guid.NewGuid().ToString();
            try
            {
                _db.ListSetByIndex(_redisKey, index, deleteFlag, CommandFlags.FireAndForget);
            }
            catch (RedisServerException redisServerException)
            {
                if (IsIndexOutOfRangeExcepiton(redisServerException))
                {
                    throw IndexOutOfRangeException;
                }
                throw;
            }
            _db.ListRemove(_redisKey, deleteFlag, flags: CommandFlags.FireAndForget);
        }

        public void Add(T item)
        {
            _db.ListRightPush(_redisKey, item.ToRedisValue());
        }

        public void Clear()
        {
            _db.KeyDelete(_redisKey);
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public int Count
        {
            get
            {
                long count = _db.ListLength(_redisKey);
                if (count > int.MaxValue)
                {
                    throw new OverflowException("Count exceeds maximum value of integer.");
                }
                return (int)count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public T this[int index]
        {
            get
            {
                try
                {
                    return _db.ListGetByIndex(_redisKey, index).To<T>();
                }
                catch (RedisServerException redisServerException)
                {
                    if (IsIndexOutOfRangeExcepiton(redisServerException))
                    {
                        throw IndexOutOfRangeException;
                    }
                    throw;
                }
            }
            set
            {
                Insert(index, value);
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
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<T>
        {
            private int _index;
            private T _current;
            private int _listSize;
            private RedisList<T> _redisList;

            public Enumerator(RedisList<T> redisList)
            {
                _redisList = redisList;
                _index = 0;
                _listSize = redisList.Count;
                _current = default(T);
            }

            public T Current
            {
                get
                {
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _listSize + 1)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }
                    return Current;
                }
            }

            public bool MoveNext()
            {
                if (_index >= _listSize)
                {
                    _index = _listSize + 1;
                    _current = default(T);
                    return false;
                }
                _current = _redisList[_index];
                ++_index;
                return true;
            }

            public void Reset()
            {
                _index = 0;
                _current = default(T);
            }

            public void Dispose()
            {
            }
        }

        private bool IsIndexOutOfRangeExcepiton(RedisServerException redisServerException)
        {
            const string RedisIndexOutOfRangeExceptionMessage = "ERR index out of range";
            return redisServerException.Message == RedisIndexOutOfRangeExceptionMessage;
        }
    }
}
