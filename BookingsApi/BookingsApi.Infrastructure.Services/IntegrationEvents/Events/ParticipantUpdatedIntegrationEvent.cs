using System;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantUpdatedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantUpdatedIntegrationEvent(Guid hearingId, Participant participant)
        {
            if (participant == null) return;
            HearingId = hearingId;
            Participant = ParticipantDtoMapper.MapToDto(participant);
        }

        public Guid HearingId { get; }
        public ParticipantDto Participant { get; }
    }
}