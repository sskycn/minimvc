using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility
{
    public class NoAccessException : CException
    {
        public NoAccessException(string message)
            : base(message)
        {

        }
    }
}
