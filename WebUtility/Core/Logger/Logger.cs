using System;
using System.Collections.Generic;

namespace Cvv.WebUtility.Core.Logging
{
    public class Logger : IDisposable
    {
        private readonly List<LoggingProvider> _loggingProviders = new List<LoggingProvider>();

        private readonly object _instanceLock = new object();

        public static Logger Default { get; private set; }

        static Logger()
        {
            Default = new Logger();
        }

        public void Dispose()
        {
            foreach (LoggingProvider provider in _loggingProviders)
                provider.Dispose();
        }

        public void AddProvider(LoggingProvider provider)
        {
            lock (_instanceLock)
                _loggingProviders.Add(provider);
        }

        public void Log(string formatString, params object[] p)
        {
            Log(LogLevel.Information, formatString, p);
        }

        public void LogException(Exception e)
        {
            LogException(LogLevel.Information, e);
        }

        public void LogException(LogLevel level, Exception e)
        {
            DateTime now = DateTime.Now;

            lock (_instanceLock)
            {
                foreach (LoggingProvider provider in _loggingProviders)
                {
                    if ((level & provider.LogLevelMask) != 0 && level >= provider.MinimumLogLevel)
                    {
                        provider.LogException(now, level, e);
                    }
                }
            }
        }

        public void Log(LogLevel level, string formatString, params object[] p)
        {
            string fmt = String.Format(formatString, p);

            DateTime now = DateTime.Now;

            lock (_instanceLock)
            {
                foreach (var provider in _loggingProviders)
                {
                    if ((level & provider.LogLevelMask) != 0 && level >= provider.MinimumLogLevel)
                    {
                        provider.LogText(now, level, fmt);
                    }
                }
            }
        }
    }
}