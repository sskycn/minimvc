using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class BlockStatement : StatementNode
    {
        private NodeCollection<StatementNode> _statements = new NodeCollection<StatementNode>();

        public BlockStatement(Token relatedToken)
            : base(relatedToken)
        {

        }

        public NodeCollection<StatementNode> Statements
        {
            get { return _statements; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            foreach (StatementNode statement in _statements)
            {
                statement.Execute(context, writer);
            }

            return null;
        }
    }
}
