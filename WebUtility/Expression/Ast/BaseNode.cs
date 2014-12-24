using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    abstract class BaseNode : INode
    {
        private Token _relatedToken;

        public BaseNode(Token relatedToken)
        {
            _relatedToken = relatedToken;
        }

        public Token RelatedToken
        {
            get { return _relatedToken; }
        }

        public virtual object Execute(IContext context, Stream writer)
        {
            throw new NotImplementedException();
        }
    }
}
