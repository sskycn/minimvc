using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ParenthesizedExpression : PrimaryExpression
    {
        private ExpressionNode _expression;

        public ParenthesizedExpression(Token relatedToken)
            : base(relatedToken)
        {

        }

        public ParenthesizedExpression(ExpressionNode expression)
            : base(expression.RelatedToken)
        {
            _expression = expression;
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
