using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class RemoveLinkedParticipantRequest
    {
        public Guid Id { get; }

        public RemoveLinkedParticipantRequest(Guid id)
        {
            Id = id;
        }
    }
}