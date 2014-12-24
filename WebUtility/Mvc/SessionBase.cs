using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Cvv.WebUtility.Mvc
{
    public interface IVisitorRecord
    {

    }

    public interface ISessionRecord
    {

    }

    public class SessionBase<S, V> : SessionBase
        where S : class, ISessionRecord
        where V : class, IVisitorRecord
    {
        private S _sessionRecord;
        private V _visitorRecord;

        public S SessionRecord
        {
            get
            {
                if (_sessionRecord == null)
                    _sessionRecord = WebAppConfig.SessionLoggingProvider.GetSessionObject<S>(SessionId);

                return _sessionRecord;
            }
        }

        public V VisitorRecord
        {
            get
            {
                if (_visitorRecord == null)
                    _visitorRecord = WebAppConfig.VisitorProvider.GetVisitorObject<V>(VisitorId);

                return _visitorRecord;
            }
        }
    }

    public class SessionBase
    {
        private readonly IHttpSessionState _httpSession = null;

        private string _sessionId = null;
        private string _visitorId = null;
        private long _userId = 0;
        private long _right = 0;

        private string _languageCode = null;
        private string _theme = null;

        public static IHttpResponse Response { get { return WebAppContext.Response; } }
        public static IHttpRequest Request { get { return WebAppContext.Request; } }
        public static IHttpServerUtility Server { get { return WebAppContext.Server; } }

        public static SessionBase CurrentSession
        {
            get { return WebAppContext.Session; }
        }

        public SessionBase()
        {
            _httpSession = HttpContextBase.Current.Session;

            SessionManager.CreateSessionProperties(this);


            if (IsNewSession)
            {
                SessionId = CreateSessionDataObject();

                this["_START_TIME_"] = DateTime.Now;

                if (VisitorId == null)
                    VisitorId = CreateVisitor();

                WebAppConfig.SessionLoggingProvider.AssignVisitorToSession(SessionId, VisitorId);

                DetermineLanguage();
            }
        }

        private void DetermineLanguage()
        {
            string lang = null;

            string langParam = WebAppContext.GetData["lang"];

            if (langParam != null && langParam.Length >= 2)
            {
                lang = langParam;
            }

            if (lang != null)
            {
                LanguageCode = lang;
            }
            else
            {
                LanguageCode = WebAppConfig.DefaultLanguageCode;
            }
        }

        public object this[string key]
        {
            get
            {
                if (WebAppConfig.SessionSerializer != null)
                    return WebAppConfig.SessionSerializer.GetSessionVariable(SessionId, key);

                return _httpSession[key];
            }
            set
            {
                if (WebAppConfig.SessionSerializer != null)
                    WebAppConfig.SessionSerializer.SetSessionVariable(SessionId, key, value);
                else
                    _httpSession[key] = value;
            }
        }

        public bool IsNewSession
        {
            get { return _httpSession.IsNewSession; }
        }

        public string InternalSessionID
        {
            get { return _httpSession.SessionID; }
        }

        private static void SetCookie(string cookieName, string cookieValue, bool permanent)
        {
            if (Response.Cookies[cookieName] != null)
                Response.Cookies[cookieName].Value = cookieValue;
            else
                Response.Cookies.Add(new HttpCookie(cookieName, cookieValue));

            if (permanent)
                Response.Cookies[cookieName].Expires = DateTime.Now.AddYears(10);
        }

        private static string GetCookie(string cookieName)
        {
            HttpCookie httpCookie = Request.Cookies[cookieName];

            if (httpCookie == null)
                return null;
            else
                return httpCookie.Value;
        }

        public string SessionId
        {
            get
            {
                if (_sessionId == null)
                    _sessionId = GetCookie("_SESSIONID_");

                return _sessionId;
            }
            set
            {
                SetCookie("_SESSIONID_", value, false);

                _sessionId = value;
            }

        }

        public string LanguageCode
        {
            get
            {
                if (_languageCode == null)
                {
                    _languageCode = GetCookie("LANG");

                    if (_languageCode == null || _languageCode.Length < 2)
                    {
                        return (string)this["_LANG_"];
                    }
                }

                return _languageCode;
            }

            set
            {
                this["_LANG_"] = value;

                SetCookie("LANG", value, true);

                _languageCode = value;
            }
        }

        public string Theme
        {
            get
            {
                if (_theme == null)
                {
                    _theme = GetCookie("THEME");

                    if (string.IsNullOrEmpty(_theme))
                    {
                        return (string)this["_THEME_"];
                    }
                }

                return _theme;
            }

            set
            {
                this["_THEME_"] = value;
                SetCookie("THEME", value, true);
                _theme = value;
            }
        }

        public string VisitorId
        {
            get
            {
                if (_visitorId == null)
                    _visitorId = GetCookie("_VISITORID_");

                return _visitorId;
            }
            set
            {
                if (value != null)
                {
                    SetCookie("_VISITORID_", value, true);

                    WebAppConfig.SessionLoggingProvider.AssignVisitorToSession(SessionId, value);
                }

                _visitorId = value;
            }
        }

        public long UserId
        {
            get
            {
                if (_userId == 0)
                {
                    if (this["_USER_ID_"] is long)
                        _userId = (long)this["_USER_ID_"];
                }

                return _userId;
            }
            internal set
            {
                this["_USER_ID_"] = value;

                if (value > 0)
                    WebAppConfig.SessionLoggingProvider.AssignUserToSession(SessionId, UserId);

                _userId = value;
            }
        }

        public long Right
        {
            get
            {
                if (_right == 0)
                {
                    if (this["_RIGHT_"] is long)
                        _right = (long)this["_RIGHT_"];
                }

                return _right;
            }
            internal set
            {
                this["_RIGHT_"] = value;

                _right = value;
            }
        }

        public DateTime LogonTime
        {
            get { return (DateTime)this["_START_TIME_"]; }
        }

        private static string CreateSessionDataObject()
        {
            return WebAppConfig.SessionLoggingProvider.CreateSession(Request.UrlReferrer == null ? "" : Request.UrlReferrer.OriginalString, Request.UserHostAddress, Request.UserAgent);
        }

        private static string CreateVisitor()
        {
            return WebAppConfig.VisitorProvider.CreateVisitor();
        }

        public virtual void OnSessionCreated()
        {

        }
    }
}
