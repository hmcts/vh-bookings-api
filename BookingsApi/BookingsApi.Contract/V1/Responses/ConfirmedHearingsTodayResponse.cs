using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Responses;

public class ConfirmedHearingsTodayResponse
{
    
    /// <summary>
    ///     Hearing Id
    /// </summary>
    public Guid Id { get; set; }
 
    /// <summary>
    ///     The date and time for a hearing
    /// </summary>
    public DateTime ScheduledDateTime { get; set; }
 
    /// <summary>
    ///     The duration of a hearing (number of minutes)
    /// </summary>
    public int ScheduledDuration { get; set; }
         
    /// <summary>
    ///     The name of the case type
    /// </summary>
    public string CaseTypeName { get; set; }
         
    /// <summary>
    /// The case name
    /// </summary>
    public string CaseName { get; set; }
         
    /// <summary>
    /// The case number
    /// </summary>
    public string CaseNumber { get; set; }
         
    /// <summary>
    ///     List of participants in hearing
    /// </summary>
    public List<ParticipantResponse> Participants { get; set; }
         
    /// <summary>
    /// Gets the endpoints for a hearing
    /// </summary>
    public List<EndpointResponse> Endpoints { get; set; }
         
    /// <summary>
    ///     Is the hearing venue Scottish
    /// </summary>
    public bool IsHearingVenueScottish { get; set; }
}