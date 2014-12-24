using System;
using System.Collections.Generic;
using System.Text;
using Cvv.WebUtility.Mvc;

namespace ComMiniMvc.Mini.WebApp.PageControllers
{
    public class Logout : PageController
    {
        public void Run(string returnUrl)
        {
            WebAppConfig.SecurityProvider.Logout(returnUrl);
        }
    }
}