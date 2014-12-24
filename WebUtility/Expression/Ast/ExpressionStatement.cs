using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ExpressionStatement : StatementNode
    {
        private ExpressionNode _expression;

        public ExpressionStatement(ExpressionNode expression)
            : base(expression.RelatedToken)
        {
            _expression = expression;
        }

        public override object Execute(IContext context, Stream writer)
        {
            return _expression.Execute(context, writer);
        }
    }
}
