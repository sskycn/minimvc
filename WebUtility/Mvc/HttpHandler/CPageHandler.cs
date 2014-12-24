using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Cvv.WebUtility.Mini;
using Cvv.WebUtility.Mvc.Provider;
using System.Reflection;

namespace Cvv.WebUtility.Mvc.HttpHandler
{
    public class CPageHandler
    {
        private readonly ControllerAction _controllerAction;

        internal CPageHandler(ControllerAction controllerAction)
        {
            _controllerAction = controllerAction;
        }

        public void ProcessRequest(HttpContextBase context)
        {
            context.Handler = this;
            WebAppContext.Reset();

            Stopwatch stopWatchRun = new Stopwatch();
            Stopwatch stopWatchRender = new Stopwatch();
            Stopwatch stopWatchTotal = new Stopwatch();

            stopWatchTotal.Start();

            Controller controller = null;

            try
            {
                bool isAccess = true;

                if (WebAppConfig.EnabledPermission)
                {
                    isAccess = WebAppConfig.SecurityProvider.CheckPermission(_controllerAction.ControllerClass.Name, _controllerAction.Method);
                }

                if (isAccess)
                {
                    stopWatchRun.Start();
                    controller = WebAppHelper.RunControllerAction(_controllerAction);
                    stopWatchRun.Stop();
                }
                else
                {
                    controller = WebAppHelper.RunNoAccessAction(_controllerAction);
                }

                if (controller != null && !string.IsNullOrEmpty(controller.ViewName))
                {
                    stopWatchRender.Start();
                    string htmlContent = controller.RenderView(_controllerAction);
                    stopWatchRender.Stop();
                    stopWatchTotal.Stop();

                    if (WebAppConfig.LoggingProvider != null)
                    {
                        WebAppConfig.LoggingProvider.LogPage(WebAppContext.Session.SessionId, WebAppContext.UrlWithoutExtension, GetQueryString(),
                                                         WebAppContext.Session.LanguageCode,
                                                         controller.LayoutName, controller.ViewName,
                                                         stopWatchRun.ElapsedMilliseconds,
                                                         stopWatchRender.ElapsedMilliseconds,
                                                         stopWatchTotal.ElapsedMilliseconds);
                    }

                    context.Response.AppendHeader("X-Powered-By", "MiniMVC.COM v" + WebAppConfig.Version);
                    context.Response.Write(htmlContent);
                }
                else
                {
                    stopWatchTotal.Stop();
                }
            }
            catch (FileNotFoundException ex)
            {
                context.Response.Write("<pre>");

                context.Response.Write(string.Format("<em><b>{0}</b></em><br/>", ex.Message));
                context.Response.Write(string.Format("<em><b>Filename: {0}</b></em><br/>", ex.FileName));

                context.Response.Write("</pre>");
            }
            catch (InvokeException ex)
            {
                context.Response.Write(string.Format("<pre><em><b>{0}</b></em><br/></pre>", ex.Message));
            }
            catch (ThreadAbortException)
            {
                if (WebAppConfig.LoggingProvider != null && controller != null)
                {
                    WebAppConfig.LoggingProvider.LogPage(WebAppContext.Session.SessionId, WebAppContext.UrlWithoutExtension, GetQueryString(),
                                                             WebAppContext.Session.LanguageCode,
                                                             controller.LayoutName, controller.ViewName,
                                                             stopWatchRun.ElapsedMilliseconds,
                                                             stopWatchRender.ElapsedMilliseconds,
                                                             stopWatchTotal.ElapsedMilliseconds);
                }

                throw;
            }
            catch (NoAccessException ex)
            {
                context.Response.Write(string.Format("<pre><em><b>{0}</b></em><br/></pre>", ex.Message));
            }
            catch (CException ex)
            {
                context.Response.Write(string.Format("<pre><em><b>{0}</b></em><br/></pre>", ex.Message));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is ThreadAbortException)
                    throw ex.InnerException;

                context.Response.Write("<pre>");

                for (Exception currentException = ex; currentException != null; currentException = currentException.InnerException)
                {
                    context.Response.Write("<em><b>" + currentException.Message + "</b></em><br/>");

                    context.Response.Write(currentException.StackTrace);
                }

                context.Response.Write("</pre>");
            }
            finally
            {
                if(stopWatchRun.IsRunning) stopWatchRun.Stop();
                if (stopWatchRender.IsRunning) stopWatchRender.Stop();
                if (stopWatchTotal.IsRunning) stopWatchTotal.Stop();

                stopWatchRun = null;
                stopWatchRender = null;
                stopWatchTotal = null;
            }
        }

        private static string GetQueryString()
        {
            string rawUrl = HttpContextBase.Current.Request.RawUrl;

            if (rawUrl.IndexOf("?") >= 0)
                return rawUrl.Substring(rawUrl.IndexOf("?") + 1);
            else
                return string.Empty;
        }
    }
}
