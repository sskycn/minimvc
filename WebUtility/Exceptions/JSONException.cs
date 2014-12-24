using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility
{
    public class JSONException : CException
    {
        public JSONException(string message)
            : base("JSONException: " + message)
        {

        }
    }
}
