using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility
{
    public class SecurityManager
    {
        private const int _hours = 24;

        public static long GetSecurity(string name)
        {
            Dictionary<string, long> rights = GetSecurities();

            long value;

            if (rights.TryGetValue(name, out value))
                return value;
            else
                return 0;
        }

        /// <summary>
        /// rule: url|right
        /// ext: my/campaigns$Edit|1
        /// </summary>
        /// <param name="rules"></param>
        public static void SaveSecurities(string[] rules)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlNode rootNode = doc.CreateElement("root");
            doc.AppendChild(rootNode);

            foreach (string rule in rules)
            {
                if (string.IsNullOrEmpty(rule))
                    continue;

                string[] data = rule.Split('|');

                if (data.Length < 2)
                    continue;

                XmlElement node = doc.CreateElement("security");

                XmlAttribute attr = doc.CreateAttribute("name");
                attr.Value = data[0];
                node.Attributes.Append(attr);
                node.InnerText = data[1];

                rootNode.AppendChild(node);
            }

            string fp = WebAppContext.Server.MapPath("~/App_Data/security.xml");

            doc.Save(fp);
        }

        public static Dictionary<string, long> GetSecurities()
        {
            string key = "_CVV_SECURITY_";

            Dictionary<string, long> resources = HttpRuntime.Cache.Get(key) as Dictionary<string, long>;

            if (resources == null)
            {
                string fp = WebAppContext.Server.MapPath("~/App_Data/security.xml");
                CacheDependency dp = new CacheDependency(fp);
                resources = new Dictionary<string, long>(StringComparer.InvariantCulture);

                XmlDocument doc = new XmlDocument();
                doc.Load(fp);

                XmlNodeList childNodes = doc.SelectSingleNode("root").ChildNodes;

                foreach (XmlNode n in childNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        if (n.Attributes["name"] == null || n.Attributes["name"].Value == null)
                            continue;

                        long val;

                        if (!resources.ContainsKey(n.Attributes["name"].Value))
                        {
                            long.TryParse(n.InnerText ?? string.Empty, out val);

                            resources.Add(n.Attributes["name"].Value, val);
                        }
                    }
                }

                HttpRuntime.Cache.Add(key, resources, dp, DateTime.Now.AddHours(_hours), Cache.NoSlidingExpiration, CacheItemPriority.High, null);

            }

            return resources;
        }
    }
}
