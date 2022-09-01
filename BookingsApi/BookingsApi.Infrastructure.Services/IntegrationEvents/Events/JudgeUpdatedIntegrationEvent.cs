using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class JudgeUpdatedIntegrationEvent: IIntegrationEvent
    {
        public JudgeUpdatedIntegrationEvent(Hearing hearing, Participant judge)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            Judge = ParticipantDtoMapper.MapToDto(judge);
        }

        public HearingDto Hearing { get; }

        public ParticipantDto Judge { get; }
    }
}