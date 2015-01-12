using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;
using System.Configuration;

namespace Cvv.WebUtility.Mvc
{
    public class ClientDataCollection
    {
        private readonly bool _post;

        public ClientDataCollection(bool post)
        {
            _post = post;
        }

        private NameValueCollection Data
        {
            get
            {
                if (_post)
                    return WebAppContext.Request.Form;
                else
                    return WebAppContext.Request.QueryString;
            }
        }

        public string[] Variables
        {
            get { return Data.AllKeys; }
        }

        public bool Has(string name)
        {
            return Data[name] != null;
        }

        public string Get(string name)
        {
            return Data[name];
        }

        public string Get(string name, string defaultValue)
        {
            return Data[name] ?? defaultValue;
        }

        public object Get(string name, Type targetType)
        {
            string stringValue = Get(name);

            if (targetType.FullName.IndexOf(".Model") != -1 && string.IsNullOrEmpty(stringValue))
            {
                ConstructorInfo[] constructors = targetType.GetConstructors();

                if (constructors.Length != 2)
                {
                    throw new JSONException("Model must be 2 constructors, first with zero parameters, another with full parameters as properties");
                }

                ConstructorInfo ctr = constructors[1];

                ParameterInfo[] parameters = ctr.GetParameters();

                object[] args = new object[parameters.Length];

                int i = 0;

                foreach (ParameterInfo parameter in parameters)
                {
                    args[i++] = WebAppConfig.DeserializeProvider.Parse(Get(parameter.Name), parameter.ParameterType);
                }

                return Activator.CreateInstance(targetType, args);
            }
            else
            {
                return WebAppConfig.DeserializeProvider.Parse(stringValue, targetType);
            }
        }

        public string this[string name]
        {
            get { return Data[name]; }
        }

        public T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        public T Get<T>(string name, T defaultValue)
        {
            if (!Has(name))
                return defaultValue;

            return (T)Get(name, typeof(T));
        }
    }
}
