using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface ISessionDataProvider
    {
        int CreateSession(string httpReferer, string httpRemoteAddress, string httpUserAgent);

        void AssignVisitorToSession(int sessionId, int visitorId);
        void AssignUserToSession(int sessionId, int userId);
        int GetVisitorIdForSessionId(int sessionId);

        int CreateVisitor();
    }
}
