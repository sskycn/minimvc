using System;
using System.Collections.Generic;
using System.Text;
using Cvv.WebUtility.Mvc;

namespace ComMiniMvc.Mini.WebApp.PageControllers
{
    public class Signin : PageController
    {
        public void Run(string returnUrl)
        {
            ClearView();
            WebAppConfig.SecurityProvider.SignIn(UrlEncode(returnUrl));
        }

        public void Authorize(string code, string redirect_uri)
        {
            ClearView();
            int rval = WebAppConfig.SecurityProvider.Authorize(code);

            if (rval > 0)
            {
                if (string.IsNullOrEmpty(redirect_uri))
                {
                    Redirect(Application.IndexPage);
                }
                else
                {
                    Redirect(redirect_uri);
                }
            }
            else
            {
                Redirect("error", "code=" + rval);
            }
        }
    }
}
