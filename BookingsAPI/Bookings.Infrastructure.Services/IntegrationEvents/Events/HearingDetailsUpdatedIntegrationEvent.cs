using Bookings.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
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