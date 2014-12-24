using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility
{
    public class ResourceManager
    {
        private const int _hours = 24;

        public static string GetSupportedLanguage(string language, string defaultLanguage)
        {
            string text = GetSupportedLanguages()[language];

            if (!string.IsNullOrEmpty(text))
                return language;

            return defaultLanguage;
        }

        public static NameValueCollection GetSupportedLanguages()
        {
            string key = "_CVV_SUPPORTED_LANGUAGES_";

            NameValueCollection supportedLanguages = HttpRuntime.Cache.Get(key) as NameValueCollection;

            if (supportedLanguages == null)
            {
                string filePath = WebAppContext.Server.MapPath("~/Languages/languages.xml");
                CacheDependency dp = new CacheDependency(filePath);
                supportedLanguages = new NameValueCollection();

                XmlDocument d = new XmlDocument();
                d.Load(filePath);

                foreach (XmlNode n in d.SelectSingleNode("root").ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        supportedLanguages.Add(n.Attributes["key"].Value, n.Attributes["name"].Value);
                    }
                }

                HttpRuntime.Cache.Add(key, supportedLanguages, dp, DateTime.Now.AddHours(_hours), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return supportedLanguages;
        }

        public static string GetString(string name)
        {
            Dictionary<string, string> resources = GetResources();

            string value;

            if (resources.TryGetValue(name, out value))
                return value;
            else
                return string.Empty;
        }

        public static Dictionary<string, string> GetResources()
        {
            string key = "_CVV_RESOURCES_";

            Dictionary<string, string> resources = HttpRuntime.Cache.Get(key) as Dictionary<string, string>;

            if (resources == null)
            {
                string fp = WebAppContext.Server.MapPath(string.Format("~/Languages/{0}/Resources.xml", WebAppContext.Session.LanguageCode));
                CacheDependency dp = new CacheDependency(fp);
                resources = new Dictionary<string, string>(StringComparer.InvariantCulture);

                XmlDocument d = new XmlDocument();
                d.Load(fp);

                foreach (XmlNode n in d.SelectSingleNode("root").ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        if (n.Attributes["name"] == null || n.Attributes["name"].Value == null)
                            continue;

                        if (!resources.ContainsKey(n.Attributes["name"].Value))
                        {
                            resources.Add(n.Attributes["name"].Value, n.InnerText);
                        }
                    }
                }

                HttpRuntime.Cache.Add(key, resources, dp, DateTime.Now.AddHours(_hours), Cache.NoSlidingExpiration, CacheItemPriority.High, null);

            }

            return resources;
        }
    }
}
