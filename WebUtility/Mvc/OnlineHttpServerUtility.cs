using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Cvv.WebUtility.Mvc
{
    public class OnlineHttpServerUtility : IHttpServerUtility
    {
        private readonly HttpServerUtility _server;

        public OnlineHttpServerUtility(HttpServerUtility server)
        {
            _server = server;
        }

        public string MapPath(string path)
        {
            return _server.MapPath(path);
        }

        public string HtmlDecode(string s)
        {
            return _server.HtmlDecode(s);
        }

        public string HtmlEncode(string s)
        {
            return _server.HtmlEncode(s);
        }

        public string UrlDecode(string s)
        {
            return _server.UrlDecode(s);
        }

        public string UrlEncode(string s)
        {
            return _server.UrlEncode(s);
        }

        public string UrlPathEncode(string s)
        {
            return _server.UrlPathEncode(s);
        }
    }
}
