using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateJudiciaryParticipantsRequestV2
    {
        /// <summary>
        ///     List of new judiciary participants
        /// </summary>
        public List<JudiciaryParticipantRequestV2> NewJudiciaryParticipants { get; set; } = new();

        /// <summary>
        ///     List of existing judiciary participants
        /// </summary>
        public List<EditableUpdateJudiciaryParticipantRequestV2> ExistingJudiciaryParticipants { get; set; } = new();

        /// <summary>
        ///     List of removed judiciary participant personal codes
        /// </summary>
        public List<string> RemovedJudiciaryParticipantPersonalCodes { get; set; } = new();
    }
}
