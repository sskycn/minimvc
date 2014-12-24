using System;
using System.Collections.Generic;
using System.Text;
using System.Web.SessionState;
using System.Web;
using System.Collections.Specialized;
using System.Collections;

namespace Cvv.WebUtility.Mvc
{
    public class OnlineHttpSessionState : IHttpSessionState
    {
        private readonly HttpSessionState _session;

        public OnlineHttpSessionState(HttpSessionState session)
        {
            _session = session;
        }

        public bool IsNewSession
        {
            get { return _session.IsNewSession; }
        }

        public SessionStateMode Mode
        {
            get { return _session.Mode; }
        }

        public bool IsCookieless
        {
            get { return _session.IsCookieless; }
        }

        public HttpCookieMode CookieMode
        {
            get { return _session.CookieMode; }
        }

        public int LCID
        {
            get { return _session.LCID; }
            set { _session.LCID = value; }
        }

        public int CodePage
        {
            get { return _session.CodePage; }
            set { _session.CodePage = value; }
        }

        public IHttpSessionState Contents
        {
            get { return new OnlineHttpSessionState(_session.Contents); }
        }

        public HttpStaticObjectsCollection StaticObjects
        {
            get { return _session.StaticObjects; }
        }

        public NameObjectCollectionBase.KeysCollection Keys
        {
            get { return _session.Keys; }
        }

        public bool IsReadOnly
        {
            get { return _session.IsReadOnly; }
        }

        public void Abandon()
        {
            _session.Abandon();
        }

        public void Add(string name, object value)
        {
            _session.Add(name, value);
        }

        public string SessionID
        {
            get { return _session.SessionID; }
        }

        public int Timeout
        {
            get { return _session.Timeout; }
            set { _session.Timeout = value; }
        }

        public object this[string name]
        {
            get { return _session[name]; }
            set { _session[name] = value; }
        }

        public object this[int index]
        {
            get { return _session[index]; }
            set { _session[index] = value; }
        }

        public void Clear()
        {
            _session.Clear();
        }

        public void RemoveAll()
        {
            _session.RemoveAll();
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }

        public void RemoveAt(int index)
        {
            _session.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            _session.CopyTo(array, index);
        }

        public int Count
        {
            get { return _session.Count; }
        }

        public object SyncRoot
        {
            get { return _session.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return _session.IsSynchronized; }
        }

        public IEnumerator GetEnumerator()
        {
            return _session.GetEnumerator();
        }

    }
}
