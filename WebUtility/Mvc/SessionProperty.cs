using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc
{
    public delegate T Creator<T>();

    public abstract class SessionProperty
    {
        private readonly string _sessionKey;

        internal SessionProperty(string sessionKey)
        {
            _sessionKey = sessionKey;
        }

        public string SessionKey
        {
            get { return _sessionKey; }
        }

        internal abstract bool CreateNew { set; }

        public void Clear()
        {
            HttpContextBase.Current.Session.Remove(_sessionKey);
        }

        public class KeyAttribute : Attribute
        {
            private readonly string _key;

            public KeyAttribute(string key)
            {
                _key = key;
            }

            public string Key
            {
                get { return _key; }
            }
        }

        public class DefaultValueAttribute : Attribute
        {
            private readonly object _defaultValue;

            public DefaultValueAttribute(object defaultValue)
            {
                _defaultValue = defaultValue;
            }

            public object Value
            {
                get { return _defaultValue; }
            }
        }

        public class AutoCreateNewAttribute : Attribute
        {
        }
    }

    public class SessionProperty<T> : SessionProperty
    {
        private readonly T _defaultValue;
        private Creator<T> _defaultValueCreator;

        public SessionProperty(string sessionKey)
            : base(sessionKey)
        {
            _defaultValue = default(T);
        }

        public SessionProperty(string sessionKey, T defaultValue)
            : base(sessionKey)
        {
            _defaultValue = defaultValue;
        }

        public SessionProperty(string sessionKey, Creator<T> defaultValueCreator)
            : base(sessionKey)
        {
            _defaultValueCreator = defaultValueCreator;
        }


        public T Value
        {
            get
            {
                if (HttpContextBase.Current.Session == null)
                    return default(T);

                if (HttpContextBase.Current.Session[SessionKey] == null)
                {
                    if (_defaultValueCreator != null)
                    {
                        T value = _defaultValueCreator();

                        Value = value;

                        return value;
                    }
                    else
                        return _defaultValue;
                }
                else
                    return (T)HttpContextBase.Current.Session[SessionKey];
            }
            set
            {
                if (HttpContextBase.Current.Session != null)
                    HttpContextBase.Current.Session[SessionKey] = value;
            }
        }

        internal override bool CreateNew
        {
            set
            {
                if (value)
                    _defaultValueCreator = delegate { return (T)Activator.CreateInstance(typeof(T)); };
                else
                    _defaultValueCreator = null;
            }
        }
    }
}
