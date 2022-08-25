using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class JudgeIntegrationEvent: IIntegrationEvent
    {
        public JudgeIntegrationEvent(Hearing hearing, Participant judge, string otherInformation)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            Judge = ParticipantDtoMapper.MapToDto(judge);
            Judge.SetOtherFieldsForNonEJudJudgeUser(otherInformation);
        }

        public HearingDto Hearing { get; }

        public ParticipantDto Judge { get; }
    }
}