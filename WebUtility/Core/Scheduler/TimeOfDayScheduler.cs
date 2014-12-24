using System;

namespace Cvv.WebUtility.Core.Scheduling
{
    public class TimeOfDayScheduler : Scheduler
    {
        private TimeSpan _timeOfDay;

        public TimeOfDayScheduler(string scheduleId, TimeSpan timeOfDay) : base(scheduleId)
        {
            _timeOfDay = timeOfDay;
        }

        public override bool ShouldRun()
        {
            DateTime lastRun = LastRun;

            if (lastRun < (TimeProvider.Now.Date.AddDays(-1) + TimeOfDay))
                lastRun = (TimeProvider.Now.Date.AddDays(-1) + TimeOfDay);

            DateTime nextRun = lastRun.Date + TimeOfDay;

            if (lastRun.TimeOfDay >= TimeOfDay)
                nextRun += new TimeSpan(24, 0, 0);

            if (TimeProvider.Now >= nextRun)
            {
                LastRun = TimeProvider.Now;
                return true;
            }

            return false;
        }

        public TimeSpan TimeOfDay
        {
            get { return _timeOfDay; }
        }
    }
}