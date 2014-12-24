using System;
using System.Collections.Generic;

namespace Cvv.WebUtility.Mini
{
    public class SafeDictionary<Key, Value>
    {
        private readonly object _padLock = new object();
        private readonly Dictionary<Key, Value> _dictionary;

        public SafeDictionary(int capacity)
        {
            _dictionary = new Dictionary<Key, Value>(capacity);
        }

        public SafeDictionary()
        {
            _dictionary = new Dictionary<Key, Value>();
        }

        public bool TryGetValue(Key key, out Value value)
        {
            lock (_padLock)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        public int Count
        {
            get
            {
                lock (_padLock)
                {
                    return _dictionary.Count;
                }
            }
        }

        public Value this[Key key]
        {
            get
            {
                lock (_padLock)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_padLock)
                {
                    _dictionary[key] = value;
                }
            }
        }

        public void Add(Key key, Value value)
        {
            lock (_padLock)
            {
                if (_dictionary.ContainsKey(key) == false)
                {
                    _dictionary.Add(key, value);
                }
            }
        }
    }
}
