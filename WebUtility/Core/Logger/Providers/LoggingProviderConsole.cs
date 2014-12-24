using System;

namespace Cvv.WebUtility.Core.Logging
{
    public class LoggingProviderConsole : LoggingProvider
    {
        public override void LogText(DateTime timeStamp, LogLevel logLevel, string s)
        {
            Console.WriteLine("{0} | {1}", FormatTime(timeStamp), s);
        }

        public override void Dispose()
        {
        }
    }
}