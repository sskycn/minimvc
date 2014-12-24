using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using Cvv.WebUtility.Mini;

namespace Cvv.WebUtility.Mvc
{
    public sealed class WebAppContext
    {
        private WebAppContext() { }

        [ThreadStatic]
        private static Controller _currentController;

        private static readonly ClientDataCollection _getParameters = new ClientDataCollection(false);
        private static readonly ClientDataCollection _postParameters = new ClientDataCollection(true);

        internal static void Reset()
        {

        }

        public static Controller CurrentController
        {
            get { return _currentController; }

            internal set { _currentController = value; }
        }

        public static HttpContextBase HttpContext
        {
            get { return HttpContextBase.Current; }
        }

        public static IHttpRequest Request
        {
            get { return HttpContextBase.Current.Request; }
        }

        public static IHttpResponse Response
        {
            get { return HttpContextBase.Current.Response; }
        }

        public static IHttpServerUtility Server
        {
            get { return HttpContextBase.Current.Server; }
        }

        public static SessionBase Session
        {
            get { return HttpContextBase.Current.SessionObject; }
        }

        public static Cache WebCache
        {
            get { return HttpContextBase.Current.Cache; }
        }

        public static ClientDataCollection GetData
        {
            get { return _getParameters; }
        }

        public static ClientDataCollection PostData
        {
            get { return _postParameters; }
        }

        internal static string UrlWithoutExtension
        {
            get { return UrlHelper.GetUrlPathWithoutExtension(Request.AppRelativeCurrentExecutionFilePath); }
        }
    }
}
