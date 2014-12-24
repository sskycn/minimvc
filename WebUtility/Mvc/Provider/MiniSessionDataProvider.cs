using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc.Provider
{
    internal class MiniSessionDataProvider : ISessionDataProvider
    {
        private readonly static object _staticLock = new object();
        private readonly Dictionary<int, int> _visitors = new Dictionary<int, int>();
        private static int _sessionId = 0;
        private static int _visitorId = 0;

        static MiniSessionDataProvider()
        {
            _sessionId = (int)(DateTime.Now - new DateTime(2010, 1, 1)).TotalSeconds;
            _visitorId = (int)(DateTime.Now - new DateTime(2011, 1, 1)).TotalSeconds;
        }

        public int CreateSession(string httpReferer, string httpRemoteAddress, string httpUserAgent)
        {
            lock (_staticLock)
                return ++_sessionId;
        }

        public void AssignVisitorToSession(int sessionId, int visitorId)
        {
            _visitors[sessionId] = visitorId;
        }

        public void AssignUserToSession(int sessionId, int userId)
        {
        }

        public int GetVisitorIdForSessionId(int sessionId)
        {
            if (_visitors.ContainsKey(sessionId))
                return _visitors[sessionId];
            else
                return 0;
        }

        public int CreateVisitor()
        {
            lock (_staticLock)
                return ++_visitorId;
        }

        public object GetSessionObject(int sessionId)
        {
            return null;
        }

        public object GetVisitorObject(int visitorId)
        {
            return null;
        }
    }
}
