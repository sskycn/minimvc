using System;
using System.Collections.Generic;
using System.Text;
using Cvv.WebUtility;

namespace ComMiniMvc.Mini.WebApp
{
    public class RestrictedController : PageController
    {
        protected override void NoAccess(string method)
        {
            if (!Session.SignedIn)
            {
                Response.Redirect(Application.SignInPage + "?returnUrl=" + UrlEncode(RawUrl));
            }
            else
            {
                Redirect("~/noaccess", "returnUrl=" + UrlEncode(RawUrl));
            }
        }
    }
}
