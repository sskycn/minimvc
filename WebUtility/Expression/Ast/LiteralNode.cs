using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    class LiteralNode : PrimaryExpression
    {
        public LiteralNode(Token relatedToken)
            : base(relatedToken)
        {

        }
    }
}
