using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class DictionaryCreationExpression : PrimaryExpression
    {
        private NodeCollection<NameExpressionNode> _expressions;

        public DictionaryCreationExpression(Token relatedToken)
            : base(relatedToken)
        {

        }

        public NodeCollection<NameExpressionNode> Expressions
        {
            get { return _expressions; }
            set { _expressions = value; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (NameExpressionNode node in _expressions)
            {
                if (dic.ContainsKey(node.Name))
                {
                    dic[node.Name] = node.Expression.Execute(context, writer);
                }
                else
                {
                    dic.Add(node.Name, node.Expression.Execute(context, writer));
                }
            }

            return dic;
        }
    }
}
