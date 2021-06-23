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

            ExistingParticipants = new List<ParticipantDto>();
            NewParticipants = new List<ParticipantDto>();
            RemovedParticipants = removedParticipants;
            LinkedParticipants = linkedParticipants;

            existingParticipants.ForEach(x => AddParticipant(ExistingParticipants, x));
            newParticipants.ForEach(x => AddParticipant(NewParticipants, x));
        }

        private void AddParticipant(IList<ParticipantDto> participants, Participant participant)
        {
            var participantDto = ParticipantDtoMapper.MapToDto(participant);
            participants.Add(participantDto);
        }
    }
}
