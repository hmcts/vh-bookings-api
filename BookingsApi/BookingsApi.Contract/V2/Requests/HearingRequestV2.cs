using System;

namespace BookingsApi.Contract.V2.Requests
{
    public class HearingRequestV2
    {
        /// <summary>
        /// The id of the hearing
        /// </summary>
        public Guid HearingId { get; set; }
        
        /// <summary>
        /// Participants for the hearing
        /// </summary>
        public UpdateHearingParticipantsRequestV2 Participants { get; set; }
        
        /// <summary>
        /// Endpoints for the hearing
        /// </summary>
        public UpdateHearingEndpointsRequestV2 Endpoints { get; set; }
        
        /// <summary>
        /// Judiciary participants for the hearing
        /// </summary>
        public UpdateJudiciaryParticipantsRequestV2 JudiciaryParticipants { get; set; }
    }
}
