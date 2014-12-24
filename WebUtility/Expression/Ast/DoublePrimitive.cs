using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class DoublePrimitive : LiteralNode
    {
        private double _value;

        public DoublePrimitive(Token relatedToken)
            : base(relatedToken)
        {
            double.TryParse(relatedToken.Data, out _value);
        }

        public override object Execute(IContext context, Stream writer)
        {
            return _value;
        }
    }
}
