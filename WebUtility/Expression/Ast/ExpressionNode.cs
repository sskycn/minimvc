using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    abstract class ExpressionNode : BaseNode
    {
        public ExpressionNode(Token relatedToken)
            : base(relatedToken)
        {

        }
    }
}
