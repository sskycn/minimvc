using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility
{
    public class InvokeException : CException
    {
        private string _methodName;

        public InvokeException(string methodName, Type type)
            : base(string.Format("Method '{0}' not exists of type '{1}'.", methodName, type.FullName))
        {
            _methodName = methodName;
        }

        public string MethodName
        {
            get { return _methodName; }
        }
    }
}
