using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class AddParticipantsToHearingRequestV2
    {
        /// <summary>
        ///     List of participants
        /// </summary>
        public List<ParticipantRequestV2> Participants { get; set; }
        
        /// <summary>
        ///     List of linked participants
        /// </summary>
        public List<LinkedParticipantRequestV2> LinkedParticipants { get; set; }
    }
}