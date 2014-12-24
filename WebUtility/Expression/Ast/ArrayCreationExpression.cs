using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ArrayCreationExpression : PrimaryExpression
    {
        private ExpressionList _expressions;

        public ArrayCreationExpression(Token relatedToken)
            : base(relatedToken)
        {
        }

        public ExpressionList Expressions
        {
            get { return _expressions; }
            set { _expressions = value; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            return _expressions.Execute(context, writer);
        }
    }
}
