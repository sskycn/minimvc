using System;
using System.Text;
using Cvv.WebUtility.Mini;
using Cvv.WebUtility.Mvc.Provider;

namespace Cvv.WebUtility.Mvc
{
    public abstract class Controller : IDisposable
    {
        protected static IHttpResponse Response { get { return WebAppContext.Response; } }
        protected static IHttpRequest Request { get { return WebAppContext.Request; } }
        protected static IHttpServerUtility Server { get { return WebAppContext.Server; } }
        protected static SessionBase Session { get { return WebAppContext.Session; } }
        protected static ClientDataCollection GetData { get { return WebAppContext.GetData; } }
        protected static ClientDataCollection PostData { get { return WebAppContext.PostData; } }
        protected static ISerializerProvider Serializer { get { return WebAppConfig.SerializeProvider; } }
        protected static IDeserializerProvider Deserializer { get { return WebAppConfig.DeserializeProvider; } }

        private ViewData _viewData;
        private string _name;
        private string _viewName;
        private string _layoutName;
        private string _innerView = null;

        public ViewData ViewData
        {
            get { return _viewData; }
        }

        protected Controller()
        {
            WebAppContext.CurrentController = this;
            _viewData = new ViewData(this);
        }

        internal void Init(ControllerClass controllerClass)
        {
            if (_viewName == null)
                _viewName = controllerClass.Name;

            _name = controllerClass.Name;
        }

        internal string Name
        {
            get { return _name; }
        }

        internal string LayoutName
        {
            get { return _layoutName; }
        }

        internal string ViewName
        {
            get { return _viewName; }
        }

        public static string RequestMethod
        {
            get { return Request.ServerVariables["REQUEST_METHOD"].ToUpper(); }
        }

        public static bool IsPost()
        {
            return RequestMethod == "POST";
        }

        public static bool IsAjax()
        {
            return Request.ServerVariables["HTTP_X_REQUESTED_WITH"] == "XMLHttpRequest";
        }

        public static string Url
        {
            get { return Request.Path; }
        }

        public static string RawUrl
        {
            get { return Request.RawUrl; }
        }

        #region View and Layout
        public void ClearView()
        {
            _viewName = string.Empty;
        }

        public void ChangeView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
                _viewName = viewName;
        }

        public void ChangeView(string viewName, string layoutName)
        {
            if (string.IsNullOrEmpty(layoutName))
                _layoutName = layoutName;

            if (string.IsNullOrEmpty(viewName))
                _viewName = viewName;
        }

        public void ClearLayout()
        {
            _layoutName = string.Empty;
        }

        public void ChangeLayout(string layoutName)
        {
            _layoutName = layoutName;
        }

        public void InnerView(string innerView)
        {
            _innerView = innerView;
        }
        #endregion

        #region Redirect
        public void Redirect(string url)
        {
            if (url.IndexOf(WebAppConfig.Extension) == -1 && url.IndexOf('?') == -1)
            {
                url = string.Concat(url, WebAppConfig.Extension);
            }

            Response.Redirect(url);
        }

        public void Redirect(string url, string methodOrQueryString)
        {
            ClearView();

            if (!url.EndsWith(WebAppConfig.Extension))
            {
                url += WebAppConfig.Extension;
            }

            if (methodOrQueryString.IndexOf('=') > -1)
            {
                url = string.Concat(url, '?', methodOrQueryString);
            }
            else
            {
                url = string.Concat(url, '/', methodOrQueryString);
            }

            Response.Redirect(url);
        }

        public void Redirect(string url, string method, string queryString)
        {
            if (!url.EndsWith(WebAppConfig.Extension))
            {
                url += WebAppConfig.Extension;
            }

            url = string.Concat(url, '/', method, '?', queryString);

            Response.Redirect(url);
        }
        #endregion

        #region Write
        protected void Write(object value)
        {
            ClearView();

            Response.ContentType = "application/json; charset=" + Response.Charset ;

            if (value == null)
            {
                Response.Write("null");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                JSONWriter.WriteValue(sb, value);
                Response.Write(sb.ToString());
            }
        }

        protected void Write(string value)
        {
            ClearView();
            Response.Write(value);
        }

        protected void Write(int value)
        {
            ClearView();
            Response.Write(value);
        }

        protected void Write(bool value)
        {
            ClearView();
            Response.Write(value);
        }
        #endregion

        protected string UrlEncode(string text)
        {
            return UrlHelper.UrlEncode(text);
        }

        protected string GetResource(string name)
        {
            return ResourceManager.GetString(name);
        }

        protected virtual void NoAccess(string method)
        {
            throw new NoAccessException("No right access to this action: " + Name + "/" + method);
        }

        internal void NoAccess(ControllerAction controllerAction)
        {
            NoAccess(controllerAction.Method);
        }

        internal string RenderView(ControllerAction controllerAction)
        {
            string method = _innerView ?? controllerAction.Method;
            Skin layout = IsAjax() ? null : WebAppHelper.GetLayout(_layoutName);
            Skin skin = WebAppHelper.GetSkin(_viewName, method);

            string outerHTML;

            ViewData["SESSION"] = WebAppContext.Session;
            ViewData["VIEWNAME"] = ViewName;
            ViewData["METHOD"] = method;
            ViewData["URL"] = Url;
            ViewData["CUR"] = Url.Substring(0, Url.LastIndexOf('/'));
            ViewData["SELF"] = string.Concat(Url, "/", method);

            ViewData["CHECK"] = new Function("Check", WebAppConfig.SecurityProvider);

            ViewData["R"] = ResourceManager.GetResources();

            if (WebAppContext.Request.ApplicationPath != "/")
                ViewData["ROOT"] = WebAppContext.Request.ApplicationPath;
            else
                ViewData["ROOT"] = "";

            ViewData["THEME"] = WebAppConfig.ThemePath;
            ViewData["EXTENSION"] = WebAppConfig.Extension;

            if (layout == null)
            {
                outerHTML = skin.RenderView(ViewData);
            }
            else
            {
                outerHTML = skin.RenderView(ViewData);
                ViewData["VIEW"] = outerHTML.TrimStart();
                outerHTML = layout.RenderView(ViewData);
            }

            return outerHTML;
        }

        public void Dispose()
        {

        }
    }
}
