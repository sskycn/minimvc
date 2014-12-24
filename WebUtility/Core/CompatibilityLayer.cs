using System;
using System.Collections.Generic;
using System.IO;

namespace Cvv.WebUtility.Core.CompatibilityLayer
{
    public static class File
    {
        public static string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }

        public static void AppendAllText(string filename, string text)
        {
            File.AppendAllText(filename, text);
        }

    }
}
