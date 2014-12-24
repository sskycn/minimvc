using System;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface IVisitorProvider
    {
        string CreateVisitor();

        T GetVisitorObject<T>(string visitorId) where T : class, IVisitorRecord;

        Type GetVisitorObjectType();
    }
}
