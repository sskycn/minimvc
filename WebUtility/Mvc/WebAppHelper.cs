using System;
using System.IO;
using System.Reflection;
using Cvv.WebUtility.Mini;
using Cvv.WebUtility.Core.Cache;

namespace Cvv.WebUtility.Mvc
{
    class WebAppHelper
    {
        private static SmartCache<string> _cache = new SmartCache<string>(1024);

        internal static SessionBase CreateSessionObject()
        {
            SessionBase session = (SessionBase)Activator.CreateInstance(WebAppConfig.SessionType);
            WebAppConfig.FireSessionCreated(session);
            return session;
        }

        /// <summary>
        /// Create parameters with ViewData cache
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="viewData"></param>
        /// <returns></returns>
        internal static object[] CreateParameters(ParameterInfo[] parameters, ViewData viewData)
        {
            object[] parameterValues = new object[parameters.Length];

            string requestMethod = WebAppContext.Request.ServerVariables["REQUEST_METHOD"].ToUpper();
            bool isPost = (requestMethod == "POST");

            object value;

            for (int i = 0; i < parameters.Length; i++)
            {
                string key = parameters[i].Name;
                Type parameterType = parameters[i].ParameterType;

                if (viewData.TryGetValue(key, out value))
                {
                    if (value.GetType() != parameterType)
                    {
                        value = GetClientValue(key, parameterType, isPost);
                        viewData[key] = value;
                    }
                }
                else
                {
                    value = GetClientValue(key, parameterType, isPost);
                    viewData.Add(key, value);
                }

                parameterValues[i] = value;
            }

            return parameterValues;
        }

        private static object GetClientValue(string parameterName, Type parameterType)
        {
            return GetClientValue(parameterName, parameterType, true);
        }

        private static object GetClientValue(string parameterName, Type parameterType, bool isPost)
        {
            object r;

            if (isPost)
            {
                r = WebAppContext.PostData.Get(parameterName, parameterType);

            }
            else
            {
                r = WebAppContext.GetData.Get(parameterName, parameterType);
            }

            return r;
        }

        internal static Controller RunControllerAction(ControllerAction controllerAction)
        {
            WebAppContext.Response.DisableCaching();

            ControllerClass controllerClass = controllerAction.ControllerClass;

            using (Controller controller = controllerClass.CreateController())
            {
                if (controllerClass.Run(controller, controllerAction.Method))
                {
                    return controller;
                }
                else
                {
                    return null;
                }
            }
        }

        internal static Controller RunNoAccessAction(ControllerAction controllerAction)
        {
            WebAppContext.Response.DisableCaching();

            ControllerClass controllerClass = controllerAction.ControllerClass;

            using (Controller controller = controllerClass.CreateController())
            {
                controller.NoAccess(controllerAction);
                return controller;
            }
        }

        internal static ControllerAction GetControllerAction(string relativePath, string pathInfo)
        {
            if (string.IsNullOrEmpty(pathInfo))
            {
                pathInfo = "Run";
            }
            else
            {
                pathInfo = pathInfo.Substring(1);
            }

            ControllerClass controllerClass = WebAppConfig.GetControllerClass(UrlHelper.GetUrlPathWithoutExtension(relativePath).ToLower());

            return controllerClass == null ? null : new ControllerAction(controllerClass, pathInfo);
        }

        internal static Skin GetSkin(string viewName)
        {
            string skinFile;

            if (!_cache.TryGetValue(viewName, out skinFile))
            {
                string skinFileName;

                if (viewName.StartsWith("~/"))
                {
                    skinFileName = string.Format("{0}/Skins/{1}.html", WebAppConfig.ThemePath, viewName.Substring(2));
                }
                else if (viewName.StartsWith("/"))
                {
                    skinFileName = string.Format("{0}/Skins/{1}.html", WebAppConfig.ThemePath, viewName.Substring(1));
                }
                else
                {
                    skinFileName = string.Format("{0}/Skins/{1}.html", WebAppConfig.ThemePath, viewName);
                }

                skinFile = WebAppContext.Server.MapPath(skinFileName);

                if (!File.Exists(skinFile))
                {
                    throw new FileNotFoundException("Unable to find the skin file.", skinFileName);
                }

                _cache.Add(viewName, skinFile);
            }

            return Skin.CreateInstance(skinFile);
        }

        internal static Skin GetSkin(string viewName, string actionMethod)
        {
            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentException("viewName");

            if (string.IsNullOrEmpty(actionMethod))
                throw new ArgumentException("actionMethod");

            string skinFile;
            string key = viewName + "|" + actionMethod;

            if (!_cache.TryGetValue(key, out skinFile))
            {
                string skinFileName;

                int pos = viewName.LastIndexOf('/');

                if (pos == -1)
                {
                    skinFileName = string.Format("{0}/Skins/_{1}/{2}.html", WebAppConfig.ThemePath, viewName, actionMethod);
                }
                else
                {
                    skinFileName = string.Format("{0}/Skins/{1}/_{2}/{3}.html", WebAppConfig.ThemePath, viewName.Substring(0, pos), viewName.Substring(pos + 1), actionMethod);
                }

                IHttpServerUtility s = WebAppContext.Server;

                if (!File.Exists(s.MapPath(skinFileName)))
                {
                    skinFileName = string.Format("{0}/Skins/{1}.html", WebAppConfig.ThemePath, viewName);

                    if (!File.Exists(s.MapPath(skinFileName)))
                    {
                        throw new FileNotFoundException("Unable to find the skin file.", skinFileName);
                    }
                }

                skinFile = s.MapPath(skinFileName);
                _cache.Add(key, skinFile);
            }

            return Skin.CreateInstance(skinFile);
        }

        internal static Skin GetLayout(string layoutName)
        {
            if (layoutName == string.Empty)
                return null;

            string skinFileName =string.Empty;

            IHttpServerUtility s = WebAppContext.Server;

            if (layoutName == null)
            {
                skinFileName = string.Format("{0}/Layouts/{1}.html", WebAppConfig.ThemePath, WebAppConfig.DefaultLayout);
            }
            else
            {
                skinFileName = string.Format("{0}/Layouts/{1}.html", WebAppConfig.ThemePath, layoutName);
            }

            string skinFile = s.MapPath(skinFileName);

            if (File.Exists(skinFile))
            {
                return Skin.CreateInstance(skinFile);
            }
            else
            {
                return null;
            }
        }
    }
}
