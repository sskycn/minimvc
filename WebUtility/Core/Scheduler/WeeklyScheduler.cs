using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Core.Scheduling
{
    public class WeeklyScheduler : TimeOfDayScheduler
    {
        private bool[] _weekDays;

        public WeeklyScheduler(string scheduleId, TimeSpan timeOfDay, params int[] daysOfWeek)
            : base(scheduleId, timeOfDay)
        {
            _weekDays = new bool[7];

            foreach (int m in daysOfWeek)
                if (m >= 0 && m < 7)
                    WeekDays[m] = true;
        }

        public override bool ShouldRun()
        {
            if (WeekDays[(int)TimeProvider.Now.DayOfWeek])
                return base.ShouldRun();

            return false;
        }

        public bool[] WeekDays
        {
            get { return _weekDays; }
        }
    }
}
