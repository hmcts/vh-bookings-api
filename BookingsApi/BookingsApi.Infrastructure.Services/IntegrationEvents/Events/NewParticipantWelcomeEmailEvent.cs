using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class NewParticipantWelcomeEmailEvent: IIntegrationEvent
    {
        public NewParticipantWelcomeEmailEvent(WelcomeEmailDto dto)
        {
            WelcomeEmail = dto;
        }
        public WelcomeEmailDto WelcomeEmail { get; set; }
    }
}
