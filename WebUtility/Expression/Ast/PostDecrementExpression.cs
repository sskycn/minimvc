using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class PostDecrementExpression : PrimaryExpression
    {
        private ExpressionNode _expression;

        public PostDecrementExpression(ExpressionNode expression)
            : base(expression.RelatedToken)
        {
            _expression = expression;
        }

        public ExpressionNode Expression
        {
            get { return _expression; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            if (_expression is IdentifierExpression)
            {
                IdentifierExpression ident = (IdentifierExpression)_expression;
                object value = ident.Execute(context, writer);

                if (Util.IsInteger(value))
                {
                    Int64 tmp = Convert.ToInt64(value) - 1;

                    if (tmp >= Int32.MinValue && tmp <= Int32.MaxValue)
                    {
                        value = (Int32)tmp;
                    }
                    else
                    {
                        value = tmp;
                    }
                }
                else
                {
                    value = Convert.ToDouble(value) - 1;
                }

                context[ident.Name] = value;
            }
            else
            {
                _expression.Execute(context, writer);
            }

            return null;
        }
    }
}
