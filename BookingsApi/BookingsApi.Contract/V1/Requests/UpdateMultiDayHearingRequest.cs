using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateMultiDayHearingRequest
    {
        /// <summary>
        ///     Updated list of participants to be in the hearing(s)
        /// </summary>
        public List<EditableParticipantRequest> Participants { get; set; } = new();
        
        // TODO endpoints
        
        /// <summary>
        ///     When true, applies updates to future days of the multi day hearing as well
        /// </summary>
        public bool UpdateFutureDays { get; set; }
    }
}
