using System;
using System.Collections.Generic;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility.Mvc.Provider
{
    internal class MiniSessionLoggingProvider : ISessionLoggingProvider
    {
        public class Session : ISessionRecord
        {

        }

        private readonly Dictionary<string, string> _visitors = new Dictionary<string, string>();

        static MiniSessionLoggingProvider()
        {

        }

        public string CreateSession(string httpReferer, string httpRemoteAddress, string httpUserAgent)
        {
            return Guid.NewGuid().ToString();
        }

        public void AssignVisitorToSession(string sessionId, string visitorId)
        {
            _visitors[sessionId] = visitorId;
        }

        public void AssignUserToSession(string sessionId, long userId)
        {
        }


        T ISessionLoggingProvider.GetSessionObject<T>(string sessionId)
        {
            return null;
        }

        public Type GetSessionObjectType()
        {
            return typeof(Session);
        }
    }
}
