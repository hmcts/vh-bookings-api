using System.Collections.Generic;
using Newtonsoft.Json;

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
        [JsonProperty("judicial_office_holders")]
        public IList<JudiciaryParticipantRequest> Participants { get; set; }
    }
}
