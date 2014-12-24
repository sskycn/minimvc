using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace Cvv.WebUtility.Mvc
{
    public interface IHttpResponse
    {
        HttpCookieCollection Cookies { get; }
        string Output { get; }
        void End();
        void Redirect(string url);
        int StatusCode { get; set; }
        string ContentType { get; set; }
        void BinaryWrite(byte[] bytes);
        void AppendHeader(string header, string value);
        void ClearHeaders();
        Stream OutputStream { get; }
        void AddCacheItemDependencies(ArrayList cacheKeys);
        void AddCacheItemDependencies(string[] cacheKeys);
        void AddCacheItemDependency(string cacheKey);
        void AddFileDependencies(ArrayList filenames);
        void AddFileDependencies(string[] filenames);
        void AddFileDependency(string filename);
        void AddHeader(string name, string value);
        void AppendCookie(HttpCookie cookie);
        void AppendToLog(string param);
        string ApplyAppPathModifier(string virtualPath);
        void Clear();
        void ClearContent();
        void Close();
        void DisableKernelCache();
        void Flush();
        void Pics(string value);
        void Redirect(string url, bool endResponse);
        void SetCookie(HttpCookie cookie);
        void TransmitFile(string filename);
        void TransmitFile(string filename, long offset, long length);
        void Write(string s);
        void Write(char ch);
        void Write(object obj);
        void Write(char[] buffer, int index, int count);
        void WriteFile(string filename);
        void WriteFile(string filename, bool readIntoMemory);
        void WriteFile(IntPtr fileHandle, long offset, long size);
        void WriteFile(string filename, long offset, long size);
        bool Buffer { get; set; }
        bool BufferOutput { get; set; }
        string CacheControl { get; set; }
        string Charset { get; set; }
        Encoding ContentEncoding { get; set; }
        int Expires { get; set; }
        DateTime ExpiresAbsolute { get; set; }
        Stream Filter { get; set; }
        Encoding HeaderEncoding { get; set; }
        NameValueCollection Headers { get; }
        bool IsClientConnected { get; }
        bool IsRequestBeingRedirected { get; }
        string RedirectLocation { get; set; }
        string Status { get; set; }
        string StatusDescription { get; set; }
        int SubStatusCode { get; set; }
        bool SuppressContent { get; set; }
        bool TrySkipIisCustomErrors { get; set; }
        void DisableCaching();
        void SetETag(string eTag);
        void SetLastModified(DateTime timeStamp);
        HttpCachePolicy Cache { get; }
    }
}
