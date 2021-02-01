using System;
using Bookings.Domain.Ddd;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Participants;

namespace Bookings.Domain
{
    public sealed class LinkedParticipant  : Entity<Guid>
    {
        public Guid ParticipantId { get; private set; }
        public Participant Participant { get; protected set; }
        public Guid LinkedId { get; private set; }
        public Participant Linked { get; protected set; }
        public LinkedParticipantType Type { get; private set; }
        
        public LinkedParticipant(Guid participantId, Guid linkedId, LinkedParticipantType type)
        {
            ParticipantId = participantId;
            LinkedId = linkedId;
            Type = type;
        }
    }
}