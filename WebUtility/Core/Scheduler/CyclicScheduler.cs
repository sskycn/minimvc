using System;

namespace Cvv.WebUtility.Core.Scheduling
{
    public class CyclicScheduler : Scheduler
    {
        private TimeSpan _interval;

        public CyclicScheduler(string scheduleId, TimeSpan interval) : base(scheduleId)
        {
            _interval = interval;
        }

        public override bool ShouldRun()
        {
            DateTime lastRun = LastRun;

            if ((TimeProvider.Now - lastRun) >= Interval)
            {
                LastRun = TimeProvider.Now;
                return true;
            }

            return false;
        }

        public TimeSpan Interval
        {
            get { return _interval; }
        }
    }
}