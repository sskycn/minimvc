using System;

namespace Cvv.WebUtility.Core.Scheduling
{
    public interface IScheduleHistoryStore
    {
        DateTime LastRun(string taskId);
        void SetLastRun(string taskId, DateTime lastRun);
    }
}