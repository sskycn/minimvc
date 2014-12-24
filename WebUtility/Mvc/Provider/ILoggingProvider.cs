using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface ILoggingProvider
    {
        int LogPage(string sessionId, string pageUrl, string queryString, string languageCode, string layoutName, string viewName, long milliSecondsRun, long milliSecondsRender, long milliSecondsTotal);
    }
}
