using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;
using Cvv.WebUtility.Mini;
using Cvv.WebUtility.Expression;

namespace Cvv.WebUtility.Mvc
{
    public class ViewData : IContext
    {
        private readonly Dictionary<string, object> _dic = new Dictionary<string, object>(StringComparer.InvariantCulture);
        private Controller _controller;

        public ViewData()
        {
            _controller = null;
        }

        internal ViewData(Controller controller)
        {
            _controller = controller;
        }

        public bool ContainsKey(string key)
        {
            return _dic.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _dic.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _dic.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            if (_dic.TryGetValue(key, out value))
                return true;

            return false;
        }

        public object this[string key]
        {
            get
            {
                object value;

                if (TryGetValue(key, out value))
                    return value;

                return null;
            }
            set { _dic[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _dic.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _dic.Values; }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)_dic).Add(item);
        }

        public void Clear()
        {
            _dic.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_dic).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_dic).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_dic).Remove(item);
        }

        public int Count
        {
            get { return _dic.Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, object>>)_dic).IsReadOnly; }
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
        }

        public Controller Controller
        {
            get { return _controller; }
        }
    }
}
