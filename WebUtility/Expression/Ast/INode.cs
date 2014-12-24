using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    interface INode
    {
        object Execute(IContext context, Stream writer);
    }
}
