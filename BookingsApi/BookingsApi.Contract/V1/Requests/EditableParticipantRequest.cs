using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class EditableParticipantRequest : ParticipantRequest
    {
        /// <summary>
        ///     Id of the participant, set to null if new participant
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     List of linked participants
        /// </summary>
        public List<LinkedParticipantRequest> LinkedParticipants { get; set; } = new();
    }
}
