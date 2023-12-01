using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class NewParticipantHearingConfirmationEvent: IIntegrationEvent
    {
        public NewParticipantHearingConfirmationEvent(HearingConfirmationForParticipantDto dto)
        {
            HearingConfirmationForParticipant = dto;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; }

    }
}
