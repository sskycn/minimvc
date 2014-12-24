using Cvv.WebUtility;
using System;
using Cvv.WebUtility.Mvc;

namespace ComMiniMvc.Mini.WebApp
{
    public class PageController : Controller
    {
        protected new MiniSession Session { get { return (MiniSession)Controller.Session; } }
    }
}
