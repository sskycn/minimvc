using Cvv.WebUtility.Mvc.Provider;

namespace ComMiniMvc.Mini.WebApp
{
    public class MiniLog : ILoggingProvider
    {
        private static ILoggingProvider _instance;

        public static ILoggingProvider CreateInstance()
        {
            if (_instance == null)
                _instance = new MiniLog();

            return _instance;
        }

        private MiniLog() { }

        #region ILoggingProvider Members

        public int LogPage(string sessionId, string pageUrl, string queryString, string languageCode, string layoutName, string viewName, long milliSecondsRun, long milliSecondsRender, long milliSecondsTotal)
        {
            return 0;
        }

        #endregion
    }
}
