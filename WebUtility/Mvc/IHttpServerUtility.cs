using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc
{
    public interface IHttpServerUtility
    {
        string MapPath(string path);

        string HtmlDecode(string s);

        string HtmlEncode(string s);

        string UrlDecode(string s);

        string UrlEncode(string s);

        string UrlPathEncode(string s);
    }
}
