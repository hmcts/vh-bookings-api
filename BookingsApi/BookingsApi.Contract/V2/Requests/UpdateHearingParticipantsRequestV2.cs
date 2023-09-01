using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateHearingParticipantsRequestV2
    {
        /// <summary>
        ///     List of new participants
        /// </summary>
        public List<ParticipantRequestV2> NewParticipants { get; set; } = new List<ParticipantRequestV2>();

        /// <summary>
        ///     List of existing participants
        /// </summary>
        public List<UpdateParticipantRequestV2> ExistingParticipants { get; set; } = new List<UpdateParticipantRequestV2>();

        /// <summary>
        ///     List of removed participant Ids
        /// </summary>
        public List<Guid> RemovedParticipantIds { get; set; } = new List<Guid>();

        /// <summary>
        ///     List of linked participants
        /// </summary>
        public List<LinkedParticipantRequestV2> LinkedParticipants { get; set; } = new List<LinkedParticipantRequestV2>();
    }
}