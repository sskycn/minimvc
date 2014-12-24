using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Cvv.WebUtility.Mvc.HttpHandler
{
    class PageHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly CPageHandler _internalHandler;

        public PageHandler()
        {

        }

        internal PageHandler(CPageHandler internalHandler)
        {
            _internalHandler = internalHandler;
        }

        public void ProcessRequest(HttpContext httpContext)
        {
            _internalHandler.ProcessRequest(HttpContextBase.Current);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
