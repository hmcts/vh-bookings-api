using System;

namespace BookingsApi.Contract.V1.Responses
{
    public class VhoWorkHoursResponse
    {
        public int DayOfWeekId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}

