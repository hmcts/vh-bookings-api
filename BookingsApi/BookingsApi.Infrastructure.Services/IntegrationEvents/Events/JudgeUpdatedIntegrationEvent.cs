using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class JudgeUpdatedIntegrationEvent: IIntegrationEvent
    {
        public JudgeUpdatedIntegrationEvent(Hearing hearing, Participant judge, bool sendNotification = true)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            Judge = ParticipantDtoMapper.MapToDto(judge);
            SendNotification = sendNotification;
        }

        public HearingDto Hearing { get; }

        public ParticipantDto Judge { get; }
        
        public bool SendNotification { get; }
    }
}