using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ForStatement: StatementNode
    {
        private NodeCollection<ExpressionNode> _inc;
        private NodeCollection<ExpressionNode> _init;
        private BlockStatement _blockStatement;
        private ExpressionNode _test;

        public ForStatement(Token relatedToken)
            : base(relatedToken)
        {
            _blockStatement = new BlockStatement(relatedToken);
            _init = new NodeCollection<ExpressionNode>();
            _inc = new NodeCollection<ExpressionNode>();
        }


        public NodeCollection<ExpressionNode> Inc
        {
            get { return _inc; }
        }

        public NodeCollection<ExpressionNode> Init
        {
            get { return _init; }
        }

        public BlockStatement BlockStatement
        {
            get { return _blockStatement; }
        }

        public ExpressionNode Test
        {
            get { return _test; }
            set { _test = value; }
        }

        private void EvalCollection(IContext context, NodeCollection<ExpressionNode> collection, Stream writer)
        {
            foreach (ExpressionNode node in collection)
            {
                node.Execute(context, writer);
            }
        }

        public override object Execute(IContext context, Stream writer)
        {
            if (_init != null)
            {
                EvalCollection(context, _init, writer);
            }

            while ((_test != null) && Util.ToBool(_test.Execute(context, writer)))
            {
                _blockStatement.Execute(context, writer);
                EvalCollection(context, _inc, writer);
            }

            return null;
        }
    }
}
