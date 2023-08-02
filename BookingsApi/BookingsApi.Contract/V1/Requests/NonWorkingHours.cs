using System;

namespace BookingsApi.Contract.V1.Requests;

public class NonWorkingHours
{
    public long Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
