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
        // TODO confirm property name with Murali
        [JsonProperty("joh")] // TODO confirm with Murali
        public IList<JudiciaryParticipantRequest> Participants { get; set; }
    }
}
