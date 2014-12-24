using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ArgumentNode : BaseNode
    {
        private ExpressionNode _expression;

        public ArgumentNode(Token relatedToken)
            : base(relatedToken)
        {
        }

        public ExpressionNode Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            return _expression.Execute(context, writer);
        }
    }
}
