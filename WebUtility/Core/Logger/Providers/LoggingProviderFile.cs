using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Cvv.WebUtility.Core.Logging
{
    public class LoggingProviderFile : LoggingProvider
    {
        private DateTime _lastCleanupTime = DateTime.MinValue;
        private readonly object _fileLock = new object();
        private StreamWriter _streamWriter;

        public string FileName { get; set; }
        public string LastUsedFileName { get; private set; }
        public bool LogRotation { get; set; }
        public bool KeepOpen { get; set; }
        public TimeSpan MaxLogAge { get; set; }

        public LoggingProviderFile(string fileName)
        {
            MaxLogAge = TimeSpan.FromDays(7);
            FileName = fileName;
        }

        private string GenerateFileName(DateTime timeStamp)
        {
            return Regex.Replace(FileName, @"\$\((?<tag>[^\)]+)\)", m =>
                 {
                     string tag = m.Groups["tag"].Value.ToLower();

                     switch (tag)
                     {
                         case "day":
                             return timeStamp.Day.ToString("00");
                         case "month":
                             return timeStamp.Month.ToString("00");
                         case "year":
                             return timeStamp.Year.ToString("0000");
                         case "min":
                             return timeStamp.Minute.ToString("00");
                         case "hour":
                             return timeStamp.Hour.ToString("00");
                         case "sec":
                             return timeStamp.Second.ToString("00");
                         case "dow":
                             return timeStamp.DayOfWeek.ToString();
                     }
                     
                     return "";
                }, RegexOptions.Singleline);
        }

        private string GenerateFileName()
        {
            return GenerateFileName(DateTime.Now);
        }

        private void RemoveOldFiles()
        {
            if (!LogRotation)
                return;

            if ((DateTime.Now - _lastCleanupTime).TotalMinutes >= 60)
            {
                _lastCleanupTime = DateTime.Now;
            }
            else
            {
                return;
            }

            TimeSpan ts = TimeSpan.MinValue;
            DateTime date = DateTime.Now.Subtract(MaxLogAge);
            
            if (FileName.ToLower().IndexOf("$(sec)") >= 0)
                ts = TimeSpan.FromSeconds(1);
            else if (FileName.ToLower().IndexOf("$(min)") >= 0)
                ts = TimeSpan.FromMinutes(1);
            else if (FileName.ToLower().IndexOf("$(hour)") >= 0)
                ts = TimeSpan.FromHours(1);
            else if (FileName.ToLower().IndexOf("$(day)") >= 0)
                ts = TimeSpan.FromDays(1);
            else if (FileName.ToLower().IndexOf("$(month)") >= 0)
                ts = TimeSpan.FromDays(28);
            else if (FileName.ToLower().IndexOf("$(year)") >= 0)
                ts = TimeSpan.FromDays(365);
            if (ts != TimeSpan.MinValue)
                for (int i = 0; i < 500; i++)
                {
                    string fn = GenerateFileName(date);

                    File.Delete(fn);
                    
                    date = date.Subtract(ts);
                }
        }

        public override void LogText(DateTime timeStamp, LogLevel logLevel, string s)
        {
            lock (_fileLock)
            {
                string fn = GenerateFileName();

                string timePart = FormatTime(timeStamp);

                s = s.Replace("\r", "").Replace("\n", "\n" + new string(' ',timePart.Length+3)).Replace("\n","\r\n");

                s = string.Format("{0} | {1}", timePart, s);

                if (KeepOpen)
                {
                    if (LastUsedFileName != fn)
                    {
                        if (_streamWriter != null)
                            _streamWriter.Close();

                        _streamWriter = File.AppendText(fn);
                    }

                    _streamWriter.WriteLine(s);
                    _streamWriter.Flush();
                }
                else
                {
                    for (int i = 0; i < 10;i++ )
                        try
                        {
                            CompatibilityLayer.File.AppendAllText(fn, s + "\r\n");

                            break;
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(100);
                        }
                }

                LastUsedFileName = fn;
                
                RemoveOldFiles();
            }
        }


        public override void Dispose()
        {
            if (_streamWriter != null)
                _streamWriter.Close();
        }
    }
}