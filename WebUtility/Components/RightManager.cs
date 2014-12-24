using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility
{
    public class RightManager
    {
        private const int _hours = 24;

        public static long GetRight(string name)
        {
            Dictionary<string, long> rights = GetRights();

            long value;

            if (rights.TryGetValue(name, out value))
                return value;
            else
                return 0;
        }

        public static IList<RightInfo> GetList()
        {
            Dictionary<string, long> rights = GetRights();

            IList<RightInfo> list = new List<RightInfo>();

            foreach (string key in rights.Keys)
            {
                list.Add(new RightInfo(key, rights[key]));
            }

            return list;
        }

        public static void SaveRights(RightInfo[] rights)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlNode rootNode = doc.CreateElement("root");
            doc.AppendChild(rootNode);

            foreach (RightInfo e in rights)
            {
                if (string.IsNullOrEmpty(e.Name))
                    continue;

                XmlElement node = doc.CreateElement("right");

                XmlAttribute attr = doc.CreateAttribute("name");
                attr.Value = e.Name;
                node.Attributes.Append(attr);
                node.InnerText = e.Value.ToString();

                rootNode.AppendChild(node);
            }

            string fp = WebAppContext.Server.MapPath("~/App_Data/right.xml");

            doc.Save(fp);
        }

        public static Dictionary<string, long> GetRights()
        {
            string key = "_CVV_RIGHTS_";

            Dictionary<string, long> resources = HttpRuntime.Cache.Get(key) as Dictionary<string, long>;

            if (resources == null)
            {
                string fp = WebAppContext.Server.MapPath("~/App_Data/right.xml");
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
