using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ExpressionList : NodeCollection<ExpressionNode>
    {
        public virtual object[] Execute(IContext context, Stream writer)
        {
            object[] arr = new object[Count];

            int i = 0;

            foreach (ExpressionNode node in this)
            {
                arr[i++] = node.Execute(context, writer);
            }

            return arr;
        }
    }
}
