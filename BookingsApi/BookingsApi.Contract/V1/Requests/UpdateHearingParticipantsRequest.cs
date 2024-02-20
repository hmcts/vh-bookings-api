using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateHearingParticipantsRequest
    {
        /// <summary>
        ///     List of new participants
        /// </summary>
        public List<ParticipantRequest> NewParticipants { get; set; } = new List<ParticipantRequest>();

        /// <summary>
        ///     List of existing participants
        /// </summary>
        public List<UpdateParticipantRequest> ExistingParticipants { get; set; } = new List<UpdateParticipantRequest>();

        /// <summary>
        ///     List of removed participant Ids
        /// </summary>
        public List<Guid> RemovedParticipantIds { get; set; } = new List<Guid>();

        /// <summary>
        ///     List of linked participants
        /// </summary>
        public List<LinkedParticipantRequest> LinkedParticipants { get; set; } = new List<LinkedParticipantRequest>();
        
        /// <summary>
        ///     Optional name of the user who made the change, uses a default if not provided
        /// </summary>
        public string UpdatedBy { get; set; }
    }
}