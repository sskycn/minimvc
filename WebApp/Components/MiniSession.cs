using System;
using System.Collections.Generic;
using System.Text;
using Cvv.WebUtility.Mvc;

namespace ComMiniMvc.Mini.WebApp
{
    public class MiniSession : SessionBase
    {
        public bool SignedIn
        {
            get { return UserId > 0; }
        }
    }
}
