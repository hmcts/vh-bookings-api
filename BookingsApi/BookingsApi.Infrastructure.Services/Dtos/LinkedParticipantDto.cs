using System;
using BookingsApi.Domain.Ddd;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class LinkedParticipantDto : Entity<Guid>
    {
        public Guid ParticipantId { get; private set; }
        public Participant Participant { get; private set; }
        public Guid LinkedId { get; private set; }
        public Participant Linked { get; private set; }
        public LinkedParticipantType Type { get; private set; }
        
        public LinkedParticipantDto(Guid participantId, Guid linkedId, LinkedParticipantType type)
        {
            Id = Guid.NewGuid();
            ParticipantId = participantId;
            LinkedId = linkedId;
            Type = type;
        }
    }
}