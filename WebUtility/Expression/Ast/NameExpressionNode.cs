using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    class NameExpressionNode : BaseNode
    {
        private string _name;
        private ExpressionNode _expression;

        public NameExpressionNode(Token relatedToken)
            : base(relatedToken)
        {
            _name = relatedToken.Data;
        }

        public NameExpressionNode(ExpressionNode expression, Token relatedToken)
            : base(relatedToken)
        {
            _name = relatedToken.Data;
            _expression = expression;
        }

        public string Name
        {
            get { return _name; }
        }

        public ExpressionNode Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }
    }
}
