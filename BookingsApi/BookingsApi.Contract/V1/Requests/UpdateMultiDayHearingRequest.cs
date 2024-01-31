using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateMultiDayHearingRequest
    {
        /// <summary>
        ///     Updated list of participants to be in the hearing(s)
        /// </summary>
        public List<EditableParticipantRequest> Participants { get; set; } = new();
        
        /// <summary>
        ///     Updated list of endpoints to be in the hearing(s)
        /// </summary>
        public List<EditableEndpointRequest> Endpoints { get; set; } = new();
        
        /// <summary>
        ///     When true, applies updates to future days of the multi day hearing as well
        /// </summary>
        public bool UpdateFutureDays { get; set; }
    }
}
