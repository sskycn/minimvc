using System;
using Cvv.WebUtility.Expression;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace Cvv.WebUtility.Mvc
{
    public class Skin
    {
        public static Skin CreateInstance(string skinFileName)
        {
            Skin skin = (Skin)HttpRuntime.Cache[skinFileName];

            if (skin == null)
            {
                skin = new Skin(skinFileName);

                CacheDependency cd = new CacheDependency(skinFileName);

                HttpRuntime.Cache.Add(skinFileName, skin, cd, DateTime.Now.AddHours(24), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }

            return skin;
        }

        public static void Remove(string skinFileName)
        {
            HttpRuntime.Cache.Remove(skinFileName);
        }

        private CompilationUnit compilationUnit;

        private Skin(string skinFileName)
        {
            IParser parser = ParserFactory.CreateParser();

            using (StreamReader sr = new StreamReader(skinFileName, WebAppConfig.TemplateFileEncoding))
            {
                string text = sr.ReadToEnd();
                compilationUnit = parser.Parse(text);
            }
        }

        public string RenderView(IContext context)
        {
            string text = string.Empty;

            using (Stream writer = new MemoryStream())
            {
                compilationUnit.Execute(context, writer);

                writer.Position = 0;

                using (StreamReader sr = new StreamReader(writer, WebAppConfig.TemplateFileEncoding))
                {
                    text = sr.ReadToEnd();
                }
            }

            return text;
        }
    }
}
