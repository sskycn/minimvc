using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class BooleanPrimitive : LiteralNode
    {
        private bool _value;

        public BooleanPrimitive(bool value, Token relatedToken)
            : base(relatedToken)
        {
            _value = value;
        }

        public override object Execute(IContext context, Stream writer)
        {
            return _value;
        }
    }
}
