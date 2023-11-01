using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class ExistingParticipantHearingConfirmationEvent: IIntegrationEvent
    {
        public ExistingParticipantHearingConfirmationEvent(HearingConfirmationForParticipantDto dto)
        {
            HearingConfirmationForParticipant = dto;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get;  }
    }
}
