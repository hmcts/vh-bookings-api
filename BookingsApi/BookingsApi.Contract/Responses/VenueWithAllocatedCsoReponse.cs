using System;

namespace BookingsApi.Contract.Responses;

public class VenueWithAllocatedCsoResponse
{
    public string HearingVenueName { get; set; }
    public JusticeUserResponse Cso { get; set; }
}