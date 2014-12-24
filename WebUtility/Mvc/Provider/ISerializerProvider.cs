using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface ISerializerProvider
    {
        string Stringify(object obj);
    }
}
