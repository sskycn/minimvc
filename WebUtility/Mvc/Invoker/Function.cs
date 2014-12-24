using System;

namespace Cvv.WebUtility.Mvc
{
    internal class Function
    {
        private string _methodName;
        private Object _dataObject;
        private Type _type;

        public Function(string methodName, Object dataObject)
        {
            _methodName = methodName;
            _dataObject = dataObject;
            _type = dataObject.GetType();
        }

        public Function(string methodName, Object dataObject, Type type)
        {
            _methodName = methodName;
            _dataObject = dataObject;
            _type = type;
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public Object DataObject
        {
            get { return _dataObject; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}
