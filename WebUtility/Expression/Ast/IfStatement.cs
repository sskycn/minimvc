using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class IfStatement : StatementNode
    {
        private ExpressionNode _test;
        private BlockStatement _statements;
        private BlockStatement _elseStatements;

        public IfStatement(Token relatedToken)
            : base(relatedToken)
        {
            _statements = new BlockStatement(relatedToken);
        }

        public ExpressionNode Test
        {
            get { return _test; }
            set { _test = value; }
        }


        public BlockStatement Statements
        {
            get { return _statements; }
        }


        public BlockStatement ElseStatements
        {
            get
            {
                if (_elseStatements == null) _elseStatements = new BlockStatement(RelatedToken);

                return _elseStatements;
            }
        }

        public override object Execute(IContext context, Stream writer)
        {
            if (_test != null && Util.ToBool(_test.Execute(context, writer)))
            {
                _statements.Execute(context, writer);
            }
            else if (_elseStatements != null)
            {
                _elseStatements.Execute(context, writer);
            }

            return null;
        }
    }
}
