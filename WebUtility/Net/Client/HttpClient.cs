using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Text.RegularExpressions;

namespace Cvv.WebUtility.Net.Client
{
    public class HttpClient
    {
        private static string _userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0";
        private static string _accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        private static string _acceptLanguage = "en-us,en;q=0.5";
        private static string _acceptEncoding = "gzip, deflate";
        private static string _acceptCharset = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";

        private const int _timeout = 120000;
        private const string _contentType = "application/x-www-form-urlencoded";
        private static Encoding _encoding = Encoding.UTF8;

        private CookieContainer _cookieContainer;
        private IWebProxy _proxy;
        private NetworkCredential _networkCredential;

        private string _referer;

        public HttpClient()
        {
            _cookieContainer = null;
        }

        public HttpClient(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer;
        }

        public HttpWebResponse GetData(string url, out string responseText)
        {
            HttpWebRequest httpWebRequest = CreateRequest(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "text/html";
            httpWebRequest.Timeout = _timeout;
            httpWebRequest.UseDefaultCredentials = true; //Resolve http 403 issue
            httpWebRequest.Accept = _accept;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.UserAgent = _userAgent;
            httpWebRequest.Headers.Add("Accept-Language", _acceptLanguage);
            httpWebRequest.Headers.Add("Accept-Encoding", _acceptEncoding);
            httpWebRequest.Headers.Add("Accept-Charset", _acceptCharset);
            httpWebRequest.Referer = Referer;
            httpWebRequest.AllowAutoRedirect = false;

            if (_cookieContainer != null)
            {
                httpWebRequest.CookieContainer = _cookieContainer;
            }

            if (_proxy != null)
            {
                httpWebRequest.Proxy = _proxy;
            }

            if (_networkCredential != null)
            {
                httpWebRequest.Credentials = _networkCredential;
            }

            return GetData(httpWebRequest, out responseText);
        }

        public HttpWebResponse GetData(HttpWebRequest httpWebRequest, out string responseText)
        {
            ServicePointManager.Expect100Continue = false; //Resolve http 417 issue

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            Encoding encoding = GetEncoding(httpWebResponse.ContentType);

            using (Stream outStream = httpWebResponse.GetResponseStream())
            {
                string contentEncoding = httpWebResponse.ContentEncoding;

                if (contentEncoding.StartsWith("gzip"))
                {
                    MemoryStream ms = new MemoryStream();

                    using (Stream stream = new GZipStream(outStream, CompressionMode.Decompress))
                    {
                        int bit = stream.ReadByte();

                        while (bit > -1)
                        {
                            ms.WriteByte((byte)bit);
                            bit = stream.ReadByte();
                        }

                        responseText = encoding.GetString(ms.ToArray());
                    }
                }
                else if (contentEncoding.StartsWith("deflate"))
                {
                    MemoryStream ms = new MemoryStream();

                    using (Stream stream = new DeflateStream(outStream, CompressionMode.Decompress))
                    {
                        int bit = stream.ReadByte();

                        while (bit > -1)
                        {
                            ms.WriteByte((byte)bit);
                            bit = stream.ReadByte();
                        }

                        responseText = encoding.GetString(ms.ToArray());
                    }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(outStream, encoding))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }
            }

            return httpWebResponse;
        }

        public HttpWebResponse GetBits(string url, string contentType, out byte[] bits)
        {
            HttpWebRequest httpWebRequest = CreateRequest(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = _timeout;
            httpWebRequest.ContentType = contentType;
            httpWebRequest.UseDefaultCredentials = true; //Resolve http 403 issue

            if (_cookieContainer != null)
            {
                httpWebRequest.CookieContainer = _cookieContainer;
            }

            if (_proxy != null)
            {
                httpWebRequest.Proxy = _proxy;
            }

            if (_networkCredential != null)
            {
                httpWebRequest.Credentials = _networkCredential;
            }

            httpWebRequest.UserAgent = _userAgent;
            httpWebRequest.Referer = Referer;

            return GetBits(httpWebRequest, out bits);
        }

        public HttpWebResponse GetBits(HttpWebRequest httpWebRequest, out byte[] bytes)
        {
            ServicePointManager.Expect100Continue = false; //Resolve http 417 issue

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (Stream outStream = httpWebResponse.GetResponseStream())
            {
                ArrayList al = new ArrayList();
                int b = outStream.ReadByte();

                while (b != -1)
                {
                    al.Add((byte)b);
                    b = outStream.ReadByte();
                }

                bytes = new byte[al.Count];

                al.CopyTo(bytes);
            }

            return httpWebResponse;
        }

        public HttpWebRequest PostData(string url, IDictionary<string, object> data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, object> item in data)
            {
                if (sb.Length > 0)
                    sb.Append('&');

                sb.Append(item.Key);
                sb.Append("=");
                sb.Append(item.Value);
            }

            return PostData(url, sb.ToString());
        }

        public HttpWebRequest PostData(string url, string data)
        {
            byte[] formData = _encoding.GetBytes(data);

            return PostData(url, formData);
        }

        public HttpWebRequest PostData(string url, byte[] formData)
        {
            ServicePointManager.Expect100Continue = false; //Resolve http 417 issue

            HttpWebRequest httpWebRequest = CreateRequest(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = _timeout;
            httpWebRequest.ContentType = _contentType;
            httpWebRequest.ContentLength = formData.Length;
            httpWebRequest.Accept = _accept;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.UserAgent = _userAgent;
            httpWebRequest.Headers.Add("Accept-Language", _acceptLanguage);
            httpWebRequest.Headers.Add("Accept-Encoding", _acceptEncoding);
            httpWebRequest.Headers.Add("Accept-Charset", _acceptCharset);
            httpWebRequest.Referer = Referer;
            httpWebRequest.AllowAutoRedirect = false;

            if (_cookieContainer != null)
            {
                httpWebRequest.CookieContainer = _cookieContainer;
            }

            if (_proxy != null)
            {
                httpWebRequest.Proxy = _proxy;
            }

            if (_networkCredential != null)
            {
                httpWebRequest.Credentials = _networkCredential;
            }

            using (Stream s = httpWebRequest.GetRequestStream())
            {
                s.Write(formData, 0, formData.Length);
            }

            return httpWebRequest;
        }

        public string Referer
        {
            get { return _referer; }
            set { _referer = value; }
        }

        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
        }

        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        public NetworkCredential NetworkCredential
        {
            get { return _networkCredential; }
            set { _networkCredential = value; }
        }

        private HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

            return httpWebRequest;
        }

        #region Static Helper method
        private static string NewDataBoundary()
        {
            Random rnd = new Random();
            StringBuilder formDataBoundary = new StringBuilder();

            formDataBoundary.Append("---------------------------");

            formDataBoundary.Append("7d");

            for (int i = 0; i < 13; i++)
            {
                formDataBoundary.AppendFormat("{0:x}", rnd.Next(11));
            }

            return formDataBoundary.ToString();
        }

        private static Encoding GetEncoding(string contentType)
        {
            Encoding encoding = null;

            if (string.IsNullOrEmpty(contentType))
            {
                encoding = Encoding.ASCII;
            }
            else
            {
                Regex r = new Regex(@"charset=(?<charset>[a-z0-9\-]*)", RegexOptions.IgnoreCase);
                Match match = r.Match(contentType);

                if (match.Success)
                {
                    string charset = match.Groups["charset"].Value;

                    if (string.IsNullOrEmpty(charset))
                    {
                        encoding = Encoding.ASCII;
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(charset);
                    }
                }
                else
                {
                    encoding = Encoding.Default;
                }
            }
            return encoding;
        }
        #endregion
    }
}
