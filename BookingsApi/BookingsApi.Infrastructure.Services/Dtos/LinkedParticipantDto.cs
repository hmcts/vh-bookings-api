using System;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class LinkedParticipantDto
    {
        public Guid ParticipantId { get; private set; }
        public Guid LinkedId { get; private set; }
        public LinkedParticipantType Type { get; private set; }
        
        public LinkedParticipantDto(Guid participantId, Guid linkedId, LinkedParticipantType type)
        {
            ParticipantId = participantId;
            LinkedId = linkedId;
            Type = type;
        }
    }
}