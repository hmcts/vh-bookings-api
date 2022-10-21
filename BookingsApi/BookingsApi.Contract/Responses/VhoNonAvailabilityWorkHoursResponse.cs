using System;

namespace BookingsApi.Contract.Responses;

public class VhoNonAvailabilityWorkHoursResponse
{
    public DateTime EndTime { get; set; }
    public DateTime StartTime { get; set; }
}