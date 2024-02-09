using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class CreateUserIntegrationEvent : IIntegrationEvent
    {
        public CreateUserIntegrationEvent(ParticipantUserDto dto)
        {
            Participant = dto;
        }
        public ParticipantUserDto Participant { get; }
    }
}
