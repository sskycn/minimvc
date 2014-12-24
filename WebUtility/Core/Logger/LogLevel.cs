using System;

namespace Cvv.WebUtility.Core.Logging
{
    [Flags]
    public enum LogLevel
    {
        Debug = 1,
        Information = 2,
        Warning = 4,
        NonCriticalError = 8,
        Error = 16,
        CriticalError = 32,
        FatalError = 64,
        None = 0,
        All = 127
    }
}