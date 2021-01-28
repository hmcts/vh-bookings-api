using System;
using Bookings.Domain.Ddd;
using Bookings.Domain.Enumerations;

namespace Bookings.Domain
{
    public sealed class LinkedParticipant  : Entity<Guid>
    {
        public Guid ParticipantId { get; set; }
        public Guid LinkedParticipantId { get; set; }
        public LinkedParticipantType Type { get; set; }

        public LinkedParticipant(Guid participantId, Guid linkedParticipantId, LinkedParticipantType type)
        {
            ParticipantId = participantId;
            LinkedParticipantId = linkedParticipantId;
            Type = type;
        }
    }
}