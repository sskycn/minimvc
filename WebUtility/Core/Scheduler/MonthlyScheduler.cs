using System;

namespace Cvv.WebUtility.Core.Scheduling
{
    public class MonthlyScheduler : TimeOfDayScheduler
    {
        private bool[] _monthDays;

        public MonthlyScheduler(string scheduleId, TimeSpan timeOfDay, params int[] daysOfMonth) : base(scheduleId, timeOfDay)
        {
            _monthDays = new bool[32];

            foreach(int m in daysOfMonth)
                if (m > 0 && m <= 31)
                    MonthDays[m] = true;
        }

        public override bool ShouldRun()
        {
            if (MonthDays[TimeProvider.Now.Day])
                return base.ShouldRun();

            return false;
        }

        public bool[] MonthDays
        {
            get { return _monthDays; }
        }
    }
}