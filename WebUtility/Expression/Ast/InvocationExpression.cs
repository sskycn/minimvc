using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Cvv.WebUtility.Mvc;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class InvocationExpression : PrimaryExpression
    {
        private ExpressionNode _leftSide;
        private NodeCollection<ArgumentNode> _argumentList;

        public InvocationExpression(ExpressionNode leftSide, NodeCollection<ArgumentNode> argumentList)
            : base(leftSide.RelatedToken)
        {
            _leftSide = leftSide;
            _argumentList = argumentList;
        }

        public ExpressionNode LeftSide
        {
            get { return _leftSide; }
        }


        public NodeCollection<ArgumentNode> ArgumentList
        {
            get { return _argumentList; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            object lValue = _leftSide.Execute(context, writer);

            if (lValue is Function)
            {
                Function np = lValue as Function;

                object result;

                object[] args = GetArgs(context, writer);

                try
                {
                    result = np.Type.InvokeMember(np.MethodName, BindingFlags.InvokeMethod, null, np.DataObject, args);
                }
                catch (MissingMethodException ex)
                {
                    throw ex;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex;
                }

                return result;
            }
            else if (lValue is IList<MethodSchema>)
            {
                object result;

                object[] args = GetArgs(context, writer);

                IList<MethodSchema> methods = lValue as IList<MethodSchema>;

                try
                {
                    result = MethodInvoker.Invoke(context.Controller, methods, args);
                }
                catch (MissingMethodException ex)
                {
                    throw ex;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex;
                }

                return result;
            }

            return null;
        }

        private object[] GetArgs(IContext context, Stream writer)
        {
            object[] args = new object[_argumentList.Count];

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = _argumentList[i].Execute(context, writer);
            }

            return args;
        }
    }
}
