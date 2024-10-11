using System.Collections.Generic;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Requests;

/// <summary>
/// The special measure request
/// </summary>
public class ScreeningRequest
{
    /// <summary>
    ///     Is the requirement for all participants or specific participants
    /// </summary>
    public ScreeningType Type { get; set; }
    
    /// <summary>
    /// A list of entity external reference ids to protect from
    /// </summary>
    public List<string> ProtectedFrom { get; set; } = [];
}