using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    public interface ILexer
    {
        TokenCollection Lex(LexReader src);
    }
}
