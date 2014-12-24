using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections;
using System.Security.Principal;

namespace Cvv.WebUtility.Mvc
{
    public class OnlineHttpContext : HttpContextBase
    {
        private readonly HttpContext _httpContext;
        private readonly OnlineHttpRequest _httpRequest;
        private readonly OnlineHttpResponse _httpResponse;
        private readonly OnlineHttpServerUtility _httpServerUtility;
        private readonly OnlineHttpSessionState _httpSessionState;

        public OnlineHttpContext(HttpContext httpContext)
        {
            _httpContext = httpContext;

            _httpRequest = new OnlineHttpRequest(httpContext.Request);
            _httpResponse = new OnlineHttpResponse(httpContext.Response);
            _httpServerUtility = new OnlineHttpServerUtility(httpContext.Server);

            if (httpContext.Session != null)
                _httpSessionState = new OnlineHttpSessionState(httpContext.Session);
        }

        public override Exception[] AllErors
        {
            get { return _httpContext.AllErrors; }
        }

        public override Cache Cache
        {
            get { return _httpContext.Cache; }
        }

        public override IHttpRequest Request
        {
            get { return _httpRequest; }
        }

        public override IHttpResponse Response
        {
            get { return _httpResponse; }
        }

        public override IHttpServerUtility Server
        {
            get { return _httpServerUtility; }
        }

        public override IHttpSessionState Session
        {
            get { return _httpSessionState; }
        }

        public override IDictionary Items
        {
            get { return _httpContext.Items; }
        }

        public override IPrincipal User
        {
            get { return _httpContext.User; }
            set { _httpContext.User = value; }
        }
    }
}
