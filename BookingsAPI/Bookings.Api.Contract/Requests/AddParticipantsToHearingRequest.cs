using System.Collections.Generic;

namespace Bookings.Api.Contract.Requests
{
    public class AddParticipantsToHearingRequest
    {
        /// <summary>
        ///     List of participants
        /// </summary>
        public List<ParticipantRequest> Participants { get; set; }
    }
}