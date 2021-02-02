using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Participants;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantsAddedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantsAddedIntegrationEvent(Guid hearingId, IEnumerable<Participant> participants)
        {
            Participants = new List<ParticipantDto>();
            HearingId = hearingId;
            participants.ToList().ForEach(AddParticipant);
        }

        public Guid HearingId { get; }

        public IList<ParticipantDto> Participants { get; }

        private void AddParticipant(Participant participant)
        {
            var participantDto = ParticipantDtoMapper.MapToDto(participant);
            Participants.Add(participantDto);
        }
    }
}