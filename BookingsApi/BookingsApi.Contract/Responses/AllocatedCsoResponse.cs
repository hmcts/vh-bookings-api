using System;

namespace BookingsApi.Contract.Responses;

public class AllocatedCsoResponse
{
    public Guid HearingId { get; set; }
    public JusticeUserResponse Cso { get; set; }
    public bool SupportsWorkAllocation { get; set; }
}