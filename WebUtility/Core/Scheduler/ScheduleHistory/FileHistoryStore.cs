using System;
using System.Collections.Generic;
using System.IO;

namespace Cvv.WebUtility.Core.Scheduling
{
    public class FileHistoryStore : IScheduleHistoryStore
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, DateTime> _lastRunTimes;
        private readonly string _fileName;

        public FileHistoryStore(string fileName)
        {
            _fileName = fileName;

            if (File.Exists(fileName))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(fileName);

                    _lastRunTimes = BinarySerializer.Deserialize<Dictionary<string, DateTime>>(bytes);

                    return;
                }
                catch // Some error when deserializing. Let's assume the data is invalid
                {
                }
            }
            
            _lastRunTimes = new Dictionary<string, DateTime>();
        }

        public DateTime LastRun(string taskId)
        {
            lock (_lock)
            {
                DateTime lastRun;

                return _lastRunTimes.TryGetValue(taskId, out lastRun) ? lastRun : DateTime.MinValue;
            }
        }

        public void SetLastRun(string taskId, DateTime lastRun)
        {
            lock (_lock)
            {
                _lastRunTimes[taskId] = lastRun;

                File.WriteAllBytes(_fileName,BinarySerializer.Serialize(_lastRunTimes));
            }
        }
    }
}