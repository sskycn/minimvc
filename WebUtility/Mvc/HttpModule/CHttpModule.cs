using System;
using System.IO;
using System.IO.Compression;
using System.Web;
using Cvv.WebUtility.Mini;
using Cvv.WebUtility.Mvc.HttpHandler;

namespace Cvv.WebUtility.Mvc.HttpModule
{
    class CHttpModule : IHttpModule
    {
        private static readonly object _contextItemKey = new object();
        private static readonly object _httpCompressedKey = new object();
        
        private class RequestData
        {
            private string _url;
            private IHttpHandler _httpHandler;

            public RequestData(string url, IHttpHandler handler)
            {
                _url = url;
                _httpHandler = handler;
            }

            public string Url
            {
                get { return _url; }
            }

            public IHttpHandler HttpHandler
            {
                get { return _httpHandler; }
            }
        }

        public void Init(HttpApplication app)
        {
            WebAppConfig.Init();

            app.PostResolveRequestCache += PostResolveRequestCache;
            app.PostMapRequestHandler += PostMapRequestHandler;
            app.PreRequestHandlerExecute += PreRequestHandlerExecute;
            app.ReleaseRequestState += HttpCompressHander;
            app.PreSendRequestHeaders += HttpCompressHander;
        }

        public void Dispose()
        {

        }

        private static void PostResolveRequestCache(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            HttpRequest request = context.Request;

            string appExecPath = request.AppRelativeCurrentExecutionFilePath;

            if (!appExecPath.EndsWith(WebAppConfig.Extension))
            {
                return;
            }

            ControllerAction controllerAction = WebAppHelper.GetControllerAction(appExecPath, request.PathInfo);

            if (controllerAction != null)
            {
                string rawUrl = context.Request.RawUrl;

                IHttpHandler httpHandler = new PageHandler(new CPageHandler(controllerAction));

                if (rawUrl.EndsWith("/"))
                {
                    string filename = context.Request.ServerVariables["SCRIPT_FILENAME"];
                    rawUrl = string.Concat(rawUrl, Path.GetFileName(filename));
                }

                context.Items[_contextItemKey] = new RequestData(UrlHelper.GetUrlWithoutPathInfo(rawUrl, request.PathInfo), httpHandler);
                context.RewritePath("~/MiniMVC.axd");
            }
        }

        private static void PostMapRequestHandler(object sender, EventArgs e)
        {
            HttpContext httpContext = ((HttpApplication)sender).Context;

            RequestData requestData = (httpContext.Items[_contextItemKey] as RequestData);

            if (requestData != null)
            {
                httpContext.RewritePath(requestData.Url);

                if (requestData.HttpHandler != null)
                    httpContext.Handler = requestData.HttpHandler;
            }
        }

        private static void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContextBase.CreateContext(HttpContext.Current);
        }

        private static void HttpCompressHander(object sender, EventArgs e)
        {
            if (!WebAppConfig.EnabledHttpCompress)
                return;

            HttpContext context = ((HttpApplication)sender).Context;

            if (!context.Items.Contains(_httpCompressedKey) && context.Handler is PageHandler)
            {
                context.Items.Add(_httpCompressedKey, null);

                HttpResponse httpResponse = context.Response;

                string acceptedEncoding = (context.Request.Headers["Accept-Encoding"] ?? "").ToLower();

                if (acceptedEncoding.Contains("gzip"))
                {
                    httpResponse.Filter = new GZipStream(httpResponse.Filter, CompressionMode.Compress);
                    httpResponse.AppendHeader("Content-Encoding", "gzip");
                }
                else if (acceptedEncoding.Contains("deflate"))
                {
                    httpResponse.Filter = new DeflateStream(httpResponse.Filter, CompressionMode.Compress);
                    httpResponse.AppendHeader("Content-Encoding", "deflate");
                }
            }
        }
    }
}
