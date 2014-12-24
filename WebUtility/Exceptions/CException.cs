using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility
{
    public class CException : Exception
    {
        public CException()
            : base()
        {

        }

        public CException(string message)
            : base(message)
        {

        }

        public CException(string message, CException innerException)
            : base(message, innerException)
        {

        }
    }
}
