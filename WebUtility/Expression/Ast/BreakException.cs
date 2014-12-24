using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    class BreakException : Exception
    {
        public BreakException() :
            base()
        {

        }

        public BreakException(string message) :
            base(message)
        {
        }
    }
}
