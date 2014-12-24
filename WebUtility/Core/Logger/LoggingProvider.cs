using System;

namespace Cvv.WebUtility.Core.Logging
{
    public abstract class LoggingProvider : IDisposable
    {
        private LogLevel _logLevelMask = LogLevel.All;
        private LogLevel _minimumLogLevel = LogLevel.Information;

        protected LoggingProvider()
        {
            TimeFormatString = "yyyy.MM.dd HH:mm:ss.ff";
        }

        public virtual LogLevel LogLevelMask
        {
            get { return _logLevelMask; }
            set { _logLevelMask = value; }
        }

        public virtual LogLevel MinimumLogLevel
        {
            get { return _minimumLogLevel; }
            set { _minimumLogLevel = value; }
        }

        public string TimeFormatString { get; set; }

        public abstract void Dispose();

        public abstract void LogText(DateTime timeStamp, LogLevel logLevel, string s);

        public virtual void LogException(DateTime timeStamp, LogLevel logLevel, Exception e)
        {
            string text = "";

            Exception innerException = e;
            
            while (innerException != null)
            {
                text += "*** Exception: " + innerException.Message + "\r\n";
                text += "*** Stacktrace: " + innerException.StackTrace + "\r\n";
            
                innerException = innerException.InnerException;
            }
            
            LogText(timeStamp, logLevel, text);
        }

        public virtual string FormatTime(DateTime time)
        {
            return time.ToString(TimeFormatString);
        }
    }
}