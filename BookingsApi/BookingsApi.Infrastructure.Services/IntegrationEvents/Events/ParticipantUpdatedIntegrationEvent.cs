using System;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantUpdatedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantUpdatedIntegrationEvent(Guid hearingId, Participant participant)
        {
            ArgumentNullException.ThrowIfNull(participant);
            ArgumentNullException.ThrowIfNull(participant.Person.ContactEmail);
            HearingId = hearingId;
            Participant = ParticipantDtoMapper.MapToDto(participant);
        }

        public ParticipantUpdatedIntegrationEvent(Guid hearingId, JudiciaryParticipant judiciaryParticipant)
        {
            ArgumentNullException.ThrowIfNull(judiciaryParticipant);
            HearingId = hearingId;
            Participant = ParticipantDtoMapper.MapToDto(judiciaryParticipant);
        }

        public Guid HearingId { get; }
        public ParticipantDto Participant { get; }
    }
}