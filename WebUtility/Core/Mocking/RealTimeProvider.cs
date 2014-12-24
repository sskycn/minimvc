using System;

namespace Cvv.WebUtility.Core
{
    public class RealTimeProvider : ITimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}