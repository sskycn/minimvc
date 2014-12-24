using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface IDeserializerProvider
    {
        object Parse(string stringValue, Type targetType);
    }
}
