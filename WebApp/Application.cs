using System;
using System.Collections.Generic;
using System.Text;
using Cvv.WebUtility;
using Cvv.WebUtility.Mvc;
using System.Configuration;

namespace ComMiniMvc.Mini.WebApp
{
    public static class Application
    {
        private static readonly string _signInPage = string.Concat("~/signin", WebAppConfig.Extension);
        private static readonly string _indexPage = string.Concat("~/index", WebAppConfig.Extension);

        public static void Init()
        {
            WebAppConfig.SessionType = typeof(MiniSession);
            WebAppConfig.SecurityType = typeof(RestrictedController);
            WebAppConfig.LoggingProvider = MiniLog.CreateInstance();
        }

        public static MiniSession Session
        {
            get { return (MiniSession)WebAppContext.Session; }
        }

        public static string SignInPage
        {
            get { return _signInPage; }
        }

        public static string IndexPage
        {
            get { return _indexPage; }
        }
    }
}
