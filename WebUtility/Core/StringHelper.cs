using System;
using System.Collections.Generic;

namespace Cvv.WebUtility.Core
{
    static class StringHelper
    {
        public static string Left(string s, int num)
        {
            return s.Substring(0, Math.Min(num, s.Length));
        }

        public static string Right(string s, int num)
        {
            return s.Substring(Math.Max(0, s.Length - num));
        }
    }
}