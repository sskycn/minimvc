using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    public class ExceptionRecord : TextOnly
    {
        public ExceptionRecord(string msg)
        {
            base.Strings.Add(msg);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
    