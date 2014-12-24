using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Collections;

namespace Cvv.WebUtility.Mvc
{
    internal class UrlHolder
    {
        private readonly string _url;
        private readonly NameValueCollection _queryString;

        public UrlHolder(string url, NameValueCollection queryString)
        {
            _url = url;
            _queryString = new NameValueCollection(queryString);
        }

        public UrlHolder RemoveAll()
        {
            _queryString.Clear();

            return this;
        }

        public UrlHolder Add(string name, string value)
        {
            _queryString.Add(name, value);

            return this;
        }

        public UrlHolder Replace(string name, string value)
        {
            _queryString.Remove(name);
            _queryString.Add(name, value);

            return this;
        }

        public UrlHolder Remove(string name)
        {
            _queryString.Remove(name);

            return this;
        }

        public override string ToString()
        {
            string url = _url;

            string q = "";

            MethodInfo baseGet = typeof(NameObjectCollectionBase).GetMethod("BaseGet", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new Type[] { typeof(Int32) }, null);

            for (int i = 0; i < _queryString.Count; i++)
            {
                string key = _queryString.GetKey(i);

                if (key.Length > 0)
                    key = HttpUtility.UrlEncode(key) + '=';

                ArrayList valueList = (ArrayList)baseGet.Invoke(_queryString, new object[] { i });

                if (valueList.Count == 0)
                {
                    if (q.Length == 0)
                        q += '?';
                    else
                        q += '&';

                    q += key;
                }
                else
                {
                    foreach (string value in valueList)
                    {
                        if (q.Length == 0)
                            q += '?';
                        else
                            q += '&';

                        q += key + HttpUtility.UrlEncode(value);
                    }
                }
            }

            return url + q;
        }
    }
}
