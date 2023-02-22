using System;

namespace BookingsApi.DAL.Dtos;

public class HearingAllocationResultDto
{
    public Guid HearingId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public int Duration { get; set; }
    public string CaseNumber { get; set; }
    public string CaseType { get; set; }
    public string AllocatedCso { get; set; }
    public bool? HasWorkHoursClash { get; set; }
    public int? ConcurrentHearingsCount { get; set; }
}