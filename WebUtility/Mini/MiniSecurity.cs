using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Cvv.WebUtility.Components;
using Cvv.WebUtility.Core.Cache;
using Cvv.WebUtility.Mvc;
using Cvv.WebUtility.Mvc.Provider;
using Cvv.WebUtility.Net.Client;

namespace Cvv.WebUtility.Mini
{
    public class MiniSecurity : ISecurityProvider
    {
        private static ISecurityProvider _instance;

        public static ISecurityProvider CreateInstance()
        {
            if (_instance == null)
                _instance = new MiniSecurity();

            return _instance;
        }

        private Dictionary<string, long> _methods = new Dictionary<string, long>();
        private SmartCache<bool> _cache = new SmartCache<bool>(1024);

        private MiniSecurity()
        {
            string[] methods = WebAppConfig.GetSecurityMethods();

            foreach (string m in methods)
            {
                _methods.Add(m, 0);
            }
        }

        public void SignIn(string returnUrl)
        {
            WebAppContext.Response.Redirect(string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}", WebAppConfig.OAuthUri, WebAppConfig.AppKey, returnUrl));
        }

        public void SignIn(long userId, long[] right)
        {
            WebAppContext.Session.UserId = userId;
            WebAppContext.Session.Right = CalculateRight(right);
        }

        public void SignIn(long userId, long right)
        {
            WebAppContext.Session.UserId = userId;
            WebAppContext.Session.Right = right;
        }

        public int Authorize(string code)
        {
            int rval = 0;

            HttpClient client = new HttpClient();

            string responseText;

            client.GetData(string.Format("{0}/Authorize?client_id={1}&code={2}", WebAppConfig.OAuthUri, WebAppConfig.AppKey, code), out responseText);

            UserData userData = WebAppConfig.DeserializeProvider.Parse(responseText, typeof(UserData)) as UserData;

            if (!userData.IsNull())
            {
                DateTime limit = new DateTime(userData.Ticks);

                if (limit > WebAppConfig.TimeProvider.Now)
                {
                    string signature = StringHelper.Sha256String(string.Concat(userData.UserId, userData.FirstName, userData.LastName, userData.Username, userData.Email, userData.Right, userData.Status, WebAppConfig.AppSecret, userData.Ticks));

                    if (signature == userData.Signature)
                    {
                        rval = 1;
                        SignIn(userData.UserId, userData.Right);
                    }
                    else
                    {
                        rval = -1;
                    }
                }
                else
                {
                    rval = -2;
                }
            }
            else
            {
                rval = -3;
            }

            return rval;
        }

        public void Logout(string returnUrl)
        {
            RemoveCache(WebAppContext.Session.UserId);

            WebAppContext.Session.UserId = 0;
            WebAppContext.Session.Right = 0;

            long ticks = WebAppConfig.TimeProvider.Now.AddMinutes(2).Ticks;

            string token = StringHelper.Sha256String(string.Concat(WebAppConfig.AppSecret, ticks));

            WebAppContext.Response.Redirect(string.Format("{0}/Logout?client_id={1}&ticks={2}&token={3}&redirect_uri={4}", WebAppConfig.OAuthUri, WebAppConfig.AppKey, ticks, token, returnUrl));
        }

        public void Logout()
        {
            RemoveCache(WebAppContext.Session.UserId);

            WebAppContext.Session.UserId = 0;
            WebAppContext.Session.Right = 0;
        }

        public long CalculateRight(long[] right)
        {
            long val = 0;

            foreach (long r in right)
            {
                val = val | r;
            }

            return val;
        }

        public bool Check(string controllerName, string actionMethod)
        {
            return CheckPermission(controllerName, actionMethod);
        }

        public bool Check(long rightSrc, long rightDist)
        {
            long val = rightSrc & rightDist;

            return val > 0;
        }

        public bool Check(long rightSrc, long rightDist1, long rightDist2)
        {
            return (rightSrc & rightDist1) > 0 || (rightSrc & rightDist2) > 0;
        }

        public bool CheckPermission(string controllerName, string actionMethod)
        {
            string key = string.Concat(controllerName, "$", actionMethod);

            long value;

            if (_methods.TryGetValue(key, out value))
            {
                long userId = WebAppContext.Session.UserId;

                string cacheKey = string.Concat(userId, ":", key);

                bool pass;

                if (!_cache.TryGetValue(cacheKey, out pass))
                {
                    long right = WebAppContext.Session.Right;

                    if (value == 0)
                    {
                        value = SecurityManager.GetSecurity(key);
                        _methods[key] = value;
                    }

                    pass = (right & value) > 0;

                    _cache.Add(key, pass);
                }

                return pass;
            }
            else
            {
                return true;
            }
        }

        public void RemoveCache(string key)
        {
            _cache.Remove(key);
        }

        public void RemoveCache(long userId)
        {
            foreach (string key in _methods.Keys)
            {
                _cache.Remove(string.Concat(userId, ":", key));
            }
        }

        public void ClearCache()
        {
            _cache.ClearCache();
        }

        public string Encrypt(string data, string publicKey)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();

            CspParameters csp = new CspParameters();
            csp.Flags = CspProviderFlags.UseMachineKeyStore;

            StringBuilder sb = new StringBuilder();

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, csp))
            {
                byte[] key = Convert.FromBase64String(publicKey);
                rsa.ImportCspBlob(key);

                byte[] dataToEncrypt = byteConverter.GetBytes(data);

                using (MemoryStream ms = new MemoryStream(dataToEncrypt))
                {
                    int bytes = 0;

                    do
                    {
                        byte[] buffer = new byte[128];
                        bytes = ms.Read(buffer, 0, 128);
                        sb.AppendLine(Convert.ToBase64String(rsa.Encrypt(buffer, false)));

                    } while (bytes > 0);
                }
            }

            return sb.ToString();
        }

        public string Decrypt(string data, string privateKey)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();

            StringBuilder sb = new StringBuilder();

            CspParameters csp = new CspParameters();
            csp.Flags = CspProviderFlags.UseMachineKeyStore;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, csp))
            {
                byte[] key = Convert.FromBase64String(privateKey);
                rsa.ImportCspBlob(key);

                using (StringReader sr = new StringReader(data))
                {
                    do
                    {
                        string line = sr.ReadLine();

                        if (string.IsNullOrEmpty(line))
                            break;

                        byte[] dataToDecrypt = Convert.FromBase64String(line);

                        sb.Append(byteConverter.GetString(rsa.Decrypt(dataToDecrypt, false)));

                    } while (true);
                }
            }

            return sb.ToString();
        }
    }
}
