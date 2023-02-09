using System;

namespace BookingsApi.Contract.Responses;

public class VenueWithAllocatedCsoReponse
{
    public string HearingVenueName { get; set; }
    public JusticeUserResponse Cso { get; set; }
}