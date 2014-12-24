using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Cvv.WebUtility.Mvc.Provider;

namespace Cvv.WebUtility.Mini
{
    internal class MFilter : IStringFilter
    {
        private static readonly Regex _stripTags = new Regex("<(.|\n)+?>", RegexOptions.Compiled);

        public bool Filter(string val)
        {
            if (string.IsNullOrEmpty(val))
                return true;
            else
                return !_stripTags.IsMatch(val);
        }
    }
}
