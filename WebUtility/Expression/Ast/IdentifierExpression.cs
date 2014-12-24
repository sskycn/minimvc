using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class IdentifierExpression : PrimaryExpression
    {
        private string _name;

        public IdentifierExpression(Token relatedToken)
            : base(relatedToken)
        {
            _name = relatedToken.Data;
        }

        public string Name
        {
            get { return _name; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            object rval = context[_name];

            return rval;
        }
    }
}
