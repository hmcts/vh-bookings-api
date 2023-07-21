using System;

namespace BookingsApi.Contract.V1.Responses
{
    public class VhoNonAvailabilityWorkHoursResponse
    {
        public long Id { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
    }
}

