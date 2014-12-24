using System;
using Cvv.WebUtility.Mvc.Provider;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility.Mvc.Provider
{
    public class MinimalVisitorProvider : IVisitorProvider
    {
        public class Visitor : IVisitorRecord
        {

        }

        public string CreateVisitor()
        {
            return Guid.NewGuid().ToString();
        }

        public T GetVisitorObject<T>(string visitorId) where T : class, IVisitorRecord
        {
            return (T)(IVisitorRecord)new Visitor();
        }

        public Type GetVisitorObjectType()
        {
            return typeof(Visitor);
        }
    }
}
