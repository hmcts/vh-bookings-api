using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingParticipantsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public List<ParticipantDto> ExistingParticipants { get; set; }
        public List<ParticipantDto> NewParticipants { get; set; }
        public List<Guid> RemovedParticipants { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; set; }

        public HearingParticipantsUpdatedIntegrationEvent(Hearing hearing, List<Participant> existingParticipants, List<Participant> newParticipants,
            List<Guid> removedParticipants, List<LinkedParticipantDto> linkedParticipants)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);

            ExistingParticipants = existingParticipants.Select(participant => ParticipantDtoMapper.MapToDto(participant)).ToList();
            NewParticipants = newParticipants.Select(participant => ParticipantDtoMapper.MapToDto(participant)).ToList();
            RemovedParticipants = removedParticipants;
            LinkedParticipants = linkedParticipants;
        }
    }
}
