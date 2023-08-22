using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    /// <summary>
    /// Adds judiciary participants to a hearing
    /// </summary>
    public class AddJudiciaryParticipantsRequest
    {
        /// <summary>
        /// The list of participants to add
        /// </summary>
        public IList<JudiciaryParticipantRequest> Participants { get; set; }
    }
}
