using System;

namespace Bookings.Api.Contract.Requests
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