using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    class ParserFactory
    {
        public static IParser CreateParser()
        {
            return new HtmlParser();
        }
    }
}
