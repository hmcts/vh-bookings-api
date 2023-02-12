using System;

namespace BookingsApi.Contract.Responses;

public class AllocatedCsoResponse
{
    public AllocatedCsoResponse(Guid hearingId) => HearingId = hearingId;
    public Guid HearingId { get; }
    public JusticeUserResponse Cso { get; set; }
}