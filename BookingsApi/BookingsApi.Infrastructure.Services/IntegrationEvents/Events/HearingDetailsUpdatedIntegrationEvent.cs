using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingDetailsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDetailsUpdatedIntegrationEvent(Hearing hearing)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
        }

        public HearingDto Hearing { get; }
    }
}