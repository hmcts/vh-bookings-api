using System;

namespace BookingsApi.Contract.V2.Requests
{
    public class HearingRequestV2
    {
        public Guid HearingId { get; set; }
        public UpdateHearingParticipantsRequestV2 Participants { get; set; }
        public UpdateHearingEndpointsRequestV2 Endpoints { get; set; }
        public UpdateJudiciaryParticipantsRequestV2 JudiciaryParticipants { get; set; }
    }
}
