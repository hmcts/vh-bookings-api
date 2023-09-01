using System;

namespace BookingsApi.Contract.V1.Responses;

public class AllocatedCsoResponse
{
    public Guid HearingId { get; set; }
    public JusticeUserResponse Cso { get; set; }
    public bool SupportsWorkAllocation { get; set; }
}