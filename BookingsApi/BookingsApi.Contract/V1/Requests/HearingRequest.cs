using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class HearingRequest
    {
        /// <summary>
        /// The id of the hearing
        /// </summary>
        public Guid HearingId { get; set; }
        
        /// <summary>
        /// Participants for the hearing
        /// </summary>
        public UpdateHearingParticipantsRequest Participants { get; set; }
        
        /// <summary>
        /// Endpoints for the hearing
        /// </summary>
        public UpdateHearingEndpointsRequest Endpoints { get; set; }
    }
}
