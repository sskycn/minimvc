using System;
using System.Collections.Generic;

namespace Cvv.WebUtility.Mvc
{
    internal class MethodInvoker
    {
        public static object Invoke(Controller controller, IList<MethodSchema> methods, object[] args)
        {
            Type[] argsType = new Type[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                    throw new MissingMethodException();

                argsType[i] = args[i].GetType();
            }

            MethodSchema currentMethod = null;

            foreach (MethodSchema m in methods)
            {
                if (m.Parameters.Length != args.Length)
                    continue;

                bool found = true;

                for (int i = 0; i < argsType.Length; i++)
                {
                    if (m.Parameters[i].ParameterType != argsType[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    currentMethod = m;
                    break;
                }
            }

            if (currentMethod != null)
            {
                return currentMethod.InvokeHandler.Invoke(controller, args);
            }
            else
            {
                throw new MissingMethodException();
            }
        }
    }
}
