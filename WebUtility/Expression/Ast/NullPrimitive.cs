using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class NullPrimitive : LiteralNode
    {
        public NullPrimitive(Token relatedToken)
            : base(relatedToken)
        {

        }

        public override object Execute(IContext context, Stream writer)
        {
            return null;
        }
    }
}
