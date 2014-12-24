using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Runtime.Serialization;

namespace Cvv.WebUtility.Mvc
{
    class ControllerClass
    {
        private readonly Type _classType;
        private readonly IList<MethodSchema> _beforeMethods = new List<MethodSchema>();
        private readonly IList<MethodSchema> _afterMethods = new List<MethodSchema>();
        private readonly IDictionary<string, MethodSchema> _publicMethods = new Dictionary<string, MethodSchema>(StringComparer.InvariantCulture);
        
        private string _name;

        public ControllerClass(Type classType, string name)
        {
            _classType = classType;

            _name = name;

            Type currentClassType = _classType;
            Stack<Type> pageTypeStack = new Stack<Type>();

            while (currentClassType != typeof(Controller) && currentClassType != null)
            {
                pageTypeStack.Push(currentClassType);

                currentClassType = currentClassType.BaseType;
            }

            while (pageTypeStack.Count > 0)
            {
                currentClassType = pageTypeStack.Pop();

                MethodInfo[] methods = currentClassType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (MethodInfo methodInfo in methods)
                {
                    if (methodInfo.IsSpecialName)
                        continue;
#if MEDIUMLEVEL
                    MethodSchema data = new MethodSchema(methodInfo.Name, methodInfo, methodInfo.GetParameters(), methodInfo.IsStatic);
#else
                    MethodSchema data = new MethodSchema(methodInfo.Name, BaseMethodInvoker.GetMethodInvoker(methodInfo), methodInfo.GetParameters(), methodInfo.IsStatic);
#endif
                    if (methodInfo.Name.StartsWith("Before"))
                    {
                        _beforeMethods.Add(data);
                    }
                    else if (methodInfo.Name.StartsWith("After"))
                    {
                        _afterMethods.Add(data);
                    }
                    else
                    {
                        if (_publicMethods.ContainsKey(methodInfo.Name))
                        {
                            _publicMethods[methodInfo.Name] = data;
                        }
                        else
                        {
                            _publicMethods.Add(methodInfo.Name, data);
                        }
                    }
                }
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public Type ClassType
        {
            get { return _classType; }
        }

        internal string[] GetMethods()
        {
            List<string> list = new List<string>();

            foreach (MethodSchema m in _publicMethods.Values)
            {
                if (list.IndexOf(m.MethodName) == -1)
                    list.Add(m.MethodName);
            }

            string[] methods = new string[list.Count];

            list.CopyTo(methods, 0);

            return methods;
        }

        public Controller CreateController()
        {
            Controller controller = (Controller)Activator.CreateInstance(_classType);
            controller.Init(this);
            return controller;
        }

        public bool Run(Controller controller, string methodName)
        {
            try
            {
                MethodSchema data;

                if (_publicMethods.TryGetValue(methodName, out data))
                {
                    foreach (MethodSchema beforeData in _beforeMethods)
                    {
                        beforeData.InvokeHandler.Invoke(controller, WebAppHelper.CreateParameters(beforeData.Parameters, controller.ViewData));
                    }

                    data.InvokeHandler.Invoke(controller, WebAppHelper.CreateParameters(data.Parameters, controller.ViewData));

                    foreach (MethodSchema afterData in _afterMethods)
                    {
                        afterData.InvokeHandler.Invoke(controller, WebAppHelper.CreateParameters(afterData.Parameters, controller.ViewData));
                    }
                }
                else
                {
                    throw new InvokeException(methodName, controller.GetType());
                }

                return true;
            }
            catch (TargetInvocationException ex)
            {

#if MEDIUMLEVEL
                throw ex.InnerException;
#else
                FieldInfo remoteStackTrace = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);

                remoteStackTrace.SetValue(ex.InnerException, ex.InnerException.StackTrace + "\r\n");

                throw ex.InnerException;
#endif
            }
        }
    }
}