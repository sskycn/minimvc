using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface ISessionSerializer
    {
        void SetSessionVariable(string sessionId, string key, object value);
        object GetSessionVariable(string sessionId, string key);
    }
}
