using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingParticipantsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public List<ParticipantDto> ExistingParticipants { get; set; }
        public List<ParticipantDto> NewParticipants { get; set; }
        public List<Guid> RemovedParticipants { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; set; }

        public HearingParticipantsUpdatedIntegrationEvent(Guid hearingId, List<Participant> existingParticipants, List<Participant> newParticipants,
            List<Guid> removedParticipants, List<LinkedParticipantDto> linkedParticipants)
        {
            HearingId = hearingId;

            ExistingParticipants = existingParticipants.Select(participant => ParticipantDtoMapper.MapToDto(participant)).ToList();
            NewParticipants = newParticipants.Select(participant => ParticipantDtoMapper.MapToDto(participant)).ToList();
            RemovedParticipants = removedParticipants;
            LinkedParticipants = linkedParticipants;
        }
    }
}
