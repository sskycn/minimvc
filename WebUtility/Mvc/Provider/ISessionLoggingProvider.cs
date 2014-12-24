using System;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface ISessionLoggingProvider
    {
        string CreateSession(string httpReferer, string httpRemoteAddress, string httpUserAgent);
        void AssignVisitorToSession(string sessionId, string visitorId);
        void AssignUserToSession(string sessionId, long userId);
        T GetSessionObject<T>(string sessionId) where T : class, ISessionRecord;
        Type GetSessionObjectType();
    }
}
