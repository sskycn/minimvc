using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility.Expression
{
    public interface IContext : IDictionary<string, object>
    {
        Controller Controller { get; }
    }
}
