using System;

namespace BookingsApi.Contract.V1.Responses;

/// <summary>
/// A minimal view of a hearing with the allocated cso and potential clashes
/// </summary>
public class HearingAllocationsResponse
{
    /// <summary>
    /// The hearing id
    /// </summary>
    public Guid HearingId { get; set; }
    
    /// <summary>
    /// The date of the hearing
    /// </summary>
    public DateTime ScheduledDateTime { get; set; }
    
    /// <summary>
    /// The duration of a hearing in minutes
    /// </summary>
    public int Duration { get; set; }
    
    /// <summary>
    /// The hearing case number
    /// </summary>
    public string CaseNumber { get; set; }
    
    /// <summary>
    /// The hearing case type
    /// </summary>
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
    /// True if the allocated user has a working hours clash
    /// </summary>
    public bool? HasWorkHoursClash { get; set; }
    
    /// <summary>
    /// True if the allocated user has a non-availability clash
    /// </summary>
    public bool? HasNonAvailabilityClash { get; set; }
    
    /// <summary>
    /// Number of concurrent hearings for this user
    /// </summary>
    public int? ConcurrentHearingsCount { get; set; }
}