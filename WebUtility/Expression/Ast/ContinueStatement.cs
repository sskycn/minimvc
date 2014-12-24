using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class ContinueStatement : StatementNode
    {
        public ContinueStatement(Token relatedToken)
            : base(relatedToken)
        {

        }

        public override object Execute(IContext context, Stream writer)
        {
            throw new ContinueException();
        }
    }
}
