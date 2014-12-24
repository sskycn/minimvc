using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class StringPrimitive : LiteralNode
    {
        public StringPrimitive(Token relatedToken)
            : base(relatedToken)
        {

        }

        public override object Execute(IContext context, Stream writer)
        {
            return RelatedToken.Data;
        }
    }
}
