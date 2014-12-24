using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;

namespace Cvv.WebUtility.Mvc
{
    public interface IHttpSessionState : ICollection
    {
        string SessionID { get; }
        int Timeout { get; set; }
        bool IsNewSession { get; }
        SessionStateMode Mode { get; }
        bool IsCookieless { get; }
        HttpCookieMode CookieMode { get; }
        int LCID { get; set; }
        int CodePage { get; set;}
        IHttpSessionState Contents { get; }
        HttpStaticObjectsCollection StaticObjects { get; }
        NameObjectCollectionBase.KeysCollection Keys { get; }
        bool IsReadOnly { get; }
        void Abandon();
        void Add(string name, object value);
        void Remove(string name);
        void RemoveAt(int index);
        void Clear();
        void RemoveAll();

        object this[string key] { get; set; }
        object this[int index] { get; set; }
    }
}
