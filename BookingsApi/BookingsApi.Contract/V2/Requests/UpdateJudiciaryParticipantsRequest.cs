using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateJudiciaryParticipantsRequest
    {
        /// <summary>
        ///     List of new judiciary participants
        /// </summary>
        public List<JudiciaryParticipantRequest> NewJudiciaryParticipants { get; set; } = new();

        /// <summary>
        ///     List of existing judiciary participants
        /// </summary>
        public List<EditableUpdateJudiciaryParticipantRequest> ExistingJudiciaryParticipants { get; set; } = new();

        /// <summary>
        ///     List of removed judiciary participant personal codes
        /// </summary>
        public List<string> RemovedJudiciaryParticipantPersonalCodes { get; set; } = new();
    }
}
