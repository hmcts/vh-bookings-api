using System.Linq;
using Bookings.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingDetailsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDetailsUpdatedIntegrationEvent(Hearing hearing)
        {
            var @case = hearing.GetCases().First(); // Does this need to be a lead case? Leadcase prop needs to be set on the domain
            Hearing = HearingDtoMapper.MapToDto(hearing);
        }

        public HearingDto Hearing { get; }
    }
}