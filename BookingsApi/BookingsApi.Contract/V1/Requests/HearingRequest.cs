using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class HearingRequest
    {
        public Guid HearingId { get; set; }
        public UpdateHearingParticipantsRequest Participants { get; set; }
        public UpdateHearingEndpointsRequest Endpoints { get; set; }
    }
}
