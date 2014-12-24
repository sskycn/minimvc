using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility
{
    public class ParseException : CException
    {
        public ParseException() :
            base()
        {

        }

        public ParseException(string message) :
            base(message)
        {

        }

        public ParseException(string message, params object[] args) :
            base(string.Format(message, args))
        {

        }
    }
}
