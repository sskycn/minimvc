using System;
using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using Cvv.WebUtility.Mvc.HttpHandler;

namespace Cvv.WebUtility.Mvc
{
    public abstract class HttpContextBase
    {
        [ThreadStatic]
        private static HttpContextBase _current;

        private CPageHandler _handler;
        private SessionBase _session;

        public abstract Exception[] AllErors { get; }
        public abstract Cache Cache { get; }
        public abstract IHttpRequest Request { get; }
        public abstract IHttpResponse Response { get; }
        public abstract IHttpServerUtility Server { get; }
        public abstract IHttpSessionState Session { get; }
        public abstract IDictionary Items { get; }
        public abstract IPrincipal User { get; set; }

        public static HttpContextBase Current
        {
            get
            {
                if (HttpContext.Current != null)
                    return (HttpContextBase)HttpContext.Current.Items["_CurrentContext_"];
                else
                    return _current;
            }
            private set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["_CurrentContext_"] = value;
                else
                    _current = value;
            }
        }

        internal CPageHandler Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        internal SessionBase SessionObject
        {
            get { return _session; }
            set { _session = value; }
        }

        public static void CreateContext(HttpContext httpContext)
        {
            WebAppConfig.Init();

            HttpContextBase context = new OnlineHttpContext(httpContext);

            Current = context;

            if (httpContext.Session != null)
            {
                context.SessionObject = WebAppHelper.CreateSessionObject();
            }
        }
    }
}
