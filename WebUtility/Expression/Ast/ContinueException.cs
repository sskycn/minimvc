using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    class ContinueException : Exception
    {
        public ContinueException() :
            base()
        {

        }

        public ContinueException(string message) :
            base(message)
        {
        }
    }
}
