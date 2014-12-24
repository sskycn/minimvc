using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class IntegralPrimitive : LiteralNode
    {
        private object _value;

        public IntegralPrimitive(Token relatedToken)
            : base(relatedToken)
        {
            Int64 value;

            Int64.TryParse(relatedToken.Data, out value);

            if (value >= Int32.MinValue && value <= Int32.MaxValue)
            {
                _value = (Int32)value;
            }
            else
            {
                _value = value;
            }
        }

        public override object Execute(IContext context, Stream writer)
        {
            return _value;
        }
    }
}
