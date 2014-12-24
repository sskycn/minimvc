using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    interface IParser
    {
        CompilationUnit Parse(string text);
    }
}
