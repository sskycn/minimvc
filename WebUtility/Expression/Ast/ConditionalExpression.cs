using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ConditionalExpression : ExpressionNode
    {
        private ExpressionNode _left;
        private ExpressionNode _right;
        private ExpressionNode _test;

        public ConditionalExpression(ExpressionNode test, ExpressionNode left, ExpressionNode right)
            : base(test.RelatedToken)
        {
            this._test = test;
            this._left = left;
            this._right = right;
        }

        public ExpressionNode Test
        {
            get { return _test; }
        }

        public ExpressionNode Left
        {
            get { return _left; }
        }

        public ExpressionNode Right
        {
            get { return _right; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            bool test = Util.ToBool(_test.Execute(context, writer));

            if (test)
            {
                return _left.Execute(context, writer);
            }
            else
            {
                return _right.Execute(context, writer);
            }
        }
    }
}
