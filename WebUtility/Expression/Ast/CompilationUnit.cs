using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class CompilationUnit : BaseNode
    {
        private NodeCollection<StatementNode> _statements = new NodeCollection<StatementNode>();

        public CompilationUnit() :
            base(new Token(TokenId.Unknow))
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
