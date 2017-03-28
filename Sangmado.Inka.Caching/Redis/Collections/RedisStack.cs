using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace Sangmado.Inka.Caching.Redis.Collections
{
    public class RedisStack<T> : IStack<T>, IEnumerable<T>, IReadOnlyCollection<T>
    {
        private const string RedisKeyTemplate = "Stack:{0}";

        private static Exception IndexOutOfRangeException = new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");

        private readonly IDatabase _db;
        private readonly string _redisKey;

        public RedisStack(IDatabase database, string name)
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

        public void Push(T item)
        {
            _db.ListRightPush(_redisKey, item.ToRedisValue());
        }

        public T Pop()
        {
            return _db.ListRightPop(_redisKey, CommandFlags.FireAndForget).To<T>();
        }

        public T Peek()
        {
            return _db.ListRange(_redisKey, -1, -1, CommandFlags.FireAndForget).FirstOrDefault().To<T>();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void Clear()
        {
            _db.KeyDelete(_redisKey);
        }

        private int IndexOf(T item)
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

        private T this[int index]
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
            private RedisStack<T> _redisStack;

            public Enumerator(RedisStack<T> redisStack)
            {
                _redisStack = redisStack;
                _index = 0;
                _listSize = redisStack.Count;
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
                _current = _redisStack[_index];
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
