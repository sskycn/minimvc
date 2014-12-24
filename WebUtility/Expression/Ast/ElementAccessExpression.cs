using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ElementAccessExpression : PrimaryExpression
    {
        private ExpressionNode _leftSide;
        private ExpressionList _expressions;

        public ElementAccessExpression(Token relatedToken)
            : base(relatedToken)
        {

        }

        public ElementAccessExpression(ExpressionNode leftSide)
            : base(leftSide.RelatedToken)
        {
            _leftSide = leftSide;
        }

        public ElementAccessExpression(ExpressionNode leftSide, ExpressionList expressions)
            : base(leftSide.RelatedToken)
        {
            _leftSide = leftSide;
            _expressions = expressions;
        }

        public ExpressionNode LeftSide
        {
            get { return _leftSide; }
        }

        public ExpressionList Expressions
        {
            get { return _expressions; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            object lValue = _leftSide.Execute(context, writer);
            object rValue = _expressions.Execute(context, writer);

            if (lValue is Array)
            {
                Array arrRight = (Array)rValue;

                if (arrRight.Length == 1)
                {
                    Array arrLeft = (Array)lValue;

                    return arrLeft.GetValue(Convert.ToInt32(arrRight.GetValue(0)));
                }
                else if (arrRight.Length > 1)
                {
                    object o = lValue;
                    for (int i = 0; i < arrRight.Length; i++)
                    {
                        if (o is Array)
                        {
                            o = ((Array)o).GetValue(Convert.ToInt32(arrRight.GetValue(i)));
                        }
                        else
                        {
                            break;
                        }
                    }

                    return o;
                }
                else
                {
                    throw new CException("Syntax error, value expected of array");
                }
            }
            else if (lValue is IDictionary)
            {
                IDictionary dic = (IDictionary)lValue;
                Array arrRight = (Array)rValue;

                if (arrRight.Length == 1)
                    return dic[arrRight.GetValue(0)];
            }
            else if (lValue is IList)
            {
                IList ls = (IList)lValue;
                Array arrRight = (Array)rValue;

                if (arrRight.Length == 1)
                    return ls[Convert.ToInt32(arrRight.GetValue(0))];
            }
            else
            {
                Type type = lValue.GetType();
                PropertyInfo property = type.GetProperty("Item");

                if (property != null)
                {
                    object[] arrRight = (object[])rValue;

                    return property.GetValue(lValue, arrRight);
                }
            }

            return null;
        }
    }
}
