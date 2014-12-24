using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Cvv.WebUtility.Mvc
{
    internal class ControllerAction
    {
        private ControllerClass _controllerClass;
        private string _method;

        public ControllerAction(ControllerClass controllerClass, string method)
        {
            _controllerClass = controllerClass;
            _method = method;
        }

        public ControllerClass ControllerClass
        {
            get { return _controllerClass; }
        }

        public string Method
        {
            get { return _method; }
        }
    }
}
