using System;

namespace BookingsApi.Contract.Requests
{
    public class NonWorkingHours
    {
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }

        public NonWorkingHours(DateTime startTime, DateTime endTime)
        {
            EndTime = endTime;
            StartTime = startTime;
        }
    }
}