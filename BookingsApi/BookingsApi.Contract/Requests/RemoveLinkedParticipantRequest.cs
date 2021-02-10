using System;

namespace BookingsApi.Contract.Requests
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