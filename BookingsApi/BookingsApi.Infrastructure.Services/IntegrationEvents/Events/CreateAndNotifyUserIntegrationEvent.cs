using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class CreateAndNotifyUserIntegrationEvent : IIntegrationEvent
    {
        public CreateAndNotifyUserIntegrationEvent(HearingConfirmationForParticipantDto dto)
        {
            HearingConfirmationForParticipant = dto;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; }
    }
}

    