﻿using System;
using System.Collections.Generic;

namespace Cvv.WebUtility.Core.Cache
{
    public class SmartCache<T>
    {
        public static DateTime Min(DateTime t1, DateTime t2)
        {
            return t1 < t2 ? t1 : t2;
        }

        private class CachedItem
        {
            public readonly string Key;
            public readonly T Value;
            public DateTime ExpirationTime;
            private readonly DateTime _absoluteExpiration;
            private readonly TimeSpan _slidingExpiration;

            public CachedItem(string key, T value, TimeSpan slidingExpiration, DateTime absoluteExpiration, ITimeProvider time)
            {
                Key = key;
                Value = value;
                
                _slidingExpiration = slidingExpiration;
                _absoluteExpiration = absoluteExpiration;

                SetAccessed(time);
            }

            public void SetAccessed(ITimeProvider time)
            {
                ExpirationTime = Min(_absoluteExpiration, time.Now.Add(_slidingExpiration));
            }
        }

        private readonly Dictionary<string, LinkedListNode<CachedItem>> _dic = new Dictionary<string, LinkedListNode<CachedItem>>();

        private readonly LinkedList<CachedItem> _keys = new LinkedList<CachedItem>();
        private readonly object _lock = new object();
        private readonly ITimeProvider _time = new RealTimeProvider();
        private static readonly TimeSpan _noSlidingExpiration = TimeSpan.FromDays(1000);
        private static readonly DateTime _noAbsoluteExpiration = DateTime.MaxValue;
        private TimeSpan _defaultSlidingExpiration = _noSlidingExpiration;
        private DateTime _defaultAbsoluteExpiration = _noAbsoluteExpiration;
        private int _cacheSize;
        private TimeSpan _cleanupInterval = TimeSpan.FromSeconds(60);
        private DateTime _nextCleanup;

        public SmartCache(int cacheSize) : this(cacheSize, new RealTimeProvider())
        {
        }

        internal SmartCache(int cacheSize, ITimeProvider timeProvider)
        {
            _cacheSize = cacheSize;
            _time = timeProvider;
            _nextCleanup = _time.Now.Add(CleanupInterval);
        }

        public int ItemCount
        {
            get
            {
                lock (_lock)
                    return _dic.Count;
            }
        }

        public int CacheSize
        {
            get
            {
                lock (_lock)
                    return _cacheSize;
            }
            set
            {
                lock (_lock)
                {
                    if (value < _cacheSize)
                    {
                        while (_dic.Count > value)
                            RemoveOldest();
                    }
                    _cacheSize = value;
                }
            }
        }

        public TimeSpan CleanupInterval
        {
            get
            {
                lock (_lock)
                    return _cleanupInterval;
            }
            set
            {
                lock (_lock)
                {
                    _cleanupInterval = value;
                    _nextCleanup = _time.Now.Add(CleanupInterval);
                }
            }
        }

        public TimeSpan DefaultSlidingExpiration
        {
            get { return _defaultSlidingExpiration; }
            set { _defaultSlidingExpiration = value; }
        }

        public DateTime DefaultAbsoluteExpiration
        {
            get { return _defaultAbsoluteExpiration; }
            set { _defaultAbsoluteExpiration = value; }
        }

        public void ClearCache()
        {
            lock (_lock)
            {
                _dic.Clear();
                _keys.Clear();
            }
        }

        public bool TryGetValue(string key, out T item, Func<T> addFunc)
        {
            return TryGetValue(key, out item, DefaultSlidingExpiration, DefaultAbsoluteExpiration, addFunc);
        }

        public bool TryGetValue(string key, out T item, TimeSpan slidingExpiration, DateTime absoluteExpiration, Func<T> addFunc)
        {
            lock (_lock)
            {
                if (TryGetValue(key, out item))
                    return true;

                item = addFunc();

                Add(key, item, slidingExpiration, absoluteExpiration);

                return true;
            }
        }

        public bool TryGetValue(string key, out T item)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (_lock)
            {
                CheckCleanup();

                LinkedListNode<CachedItem> node;

                if (_dic.TryGetValue(key, out node))
                {
                    if (node.Value.ExpirationTime < _time.Now)
                    {
                        Remove(node);
                        
                        item = default(T);

                        return false;
                    }

                    Promote(key);

                    item = node.Value.Value;
                    
                    return true;
                }
            }

            item = default(T);
            
            return false;
        }

        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            lock (_lock)
            {
                LinkedListNode<CachedItem> node;

                if (_dic.TryGetValue(key, out node))
                {
                    Remove(node);
                }
            }
        }

        // make sure we have a lock when calling this
        // also make sure the item is in the cache
        private void Promote(string key)
        {
            LinkedListNode<CachedItem> node = _dic[key];

            _keys.Remove(node);
            _keys.AddFirst(node);

            node.Value.SetAccessed(_time);
        }

        // make sure we have a lock when calling this
        private void RemoveOldest()
        {
            Remove(_keys.Last);
        }

        // make sure we have a lock when calling this
        private void Remove(LinkedListNode<CachedItem> node)
        {
            _keys.Remove(node);
            _dic.Remove(node.Value.Key);
        }

        // make sure we have a writer lock when calling this
        private void CheckCleanup()
        {
            if (_time.Now < _nextCleanup)
                return;

            LinkedListNode<CachedItem> node = _keys.First;

            while (node != null)
            {
                LinkedListNode<CachedItem> next = node.Next;

                if (node.Value.ExpirationTime < _time.Now)
                    Remove(node);

                node = next;
            }

            _nextCleanup = _time.Now.Add(CleanupInterval);
        }

        public void Add(string key, T item)
        {
            Add(key, item, DefaultSlidingExpiration, DefaultAbsoluteExpiration);
        }

        public void Add(string key, T item, TimeSpan slidingExpiration)
        {
            Add(key, item, slidingExpiration, DefaultAbsoluteExpiration);
        }

        public void Add(string key, T item, DateTime absoluteExpiration)
        {
            Add(key, item, DefaultSlidingExpiration, absoluteExpiration);
        }

        public void Add(string key, T item, TimeSpan slidingExpiration, DateTime absoluteExpiration)
        {
            lock (_lock)
            {
                LinkedListNode<CachedItem> node;

                var cachedItem = new CachedItem(key, item, slidingExpiration, absoluteExpiration, _time);

                if (_dic.TryGetValue(key, out node))
                {
                    node.Value = cachedItem;

                    Promote(key);
                    
                    return;
                }

                node = new LinkedListNode<CachedItem>(cachedItem);
                
                if (_dic.Count >= _cacheSize)
                    RemoveOldest();

                _keys.AddFirst(node);

                _dic[key] = node;
            }
        }
    }
}