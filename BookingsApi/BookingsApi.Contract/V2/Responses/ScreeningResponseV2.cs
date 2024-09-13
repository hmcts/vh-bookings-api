using System;
using System.Collections.Generic;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Responses;

public class ScreeningResponseV2
{
    /// <summary>
    ///     Is the requirement for all participants or specific participants
    /// </summary>
    public ScreeningType Type { get; set; }
    
    /// <summary>
    ///     A list of participant ids to be protected from
    /// </summary>
    public List<Guid> ProtectFromParticipantsIds { get; set; } = [];
    
    /// <summary>
    ///     A list of endpoint ids to be protected from
    /// </summary>
    public List<Guid> ProtectFromEndpointsIds { get; set; } = [];
}