using System;

namespace BookingsApi.DAL.Dtos;

public class HearingAllocationResultDto
{
    public Guid HearingId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public int Duration { get; set; }
    public string CaseNumber { get; set; }
    public string CaseType { get; set; }
    
    /// <summary>
    /// The allocated CSO. Can be one of following:
    /// <list type="bullet">
    ///     <item>"Not Allocated"</item>
    ///     <item>"Not Required" (if venue is scottish or case type is generic)</item>
    ///     <item>The username of the allocated justice user</item>
    /// </list>
    /// </summary>
    public string AllocatedCso { get; set; }
    
    /// <summary>
    /// Returns whether or not there is a working hours clash
    /// Null when there is no allocated cso.
    /// </summary>
    public bool? HasWorkHoursClash { get; set; }
}