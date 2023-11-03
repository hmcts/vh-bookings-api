using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class CreateAndNotifyUserIntegrationEvent : IIntegrationEvent
    {
        public CreateAndNotifyUserIntegrationEvent(Hearing hearing, IEnumerable<Participant> participants)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);

            Participants = participants.Select(ParticipantDtoMapper.MapToDto).ToList();
            var judiciaryParticipants = hearing.JudiciaryParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
            Participants.AddRange(judiciaryParticipants);
        }

        public HearingDto Hearing { get; }
        public List<ParticipantDto> Participants { get; }
    }
}

    