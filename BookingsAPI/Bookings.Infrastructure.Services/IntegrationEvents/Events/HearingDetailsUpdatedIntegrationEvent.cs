using System.Linq;
using Bookings.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingDetailsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDetailsUpdatedIntegrationEvent(Hearing hearing)
        {
            var @case = hearing.GetCases().Single(x => x.IsLeadCase);
            Hearing = new HearingDto(hearing.Id, hearing.ScheduledDateTime, hearing.ScheduledDuration,
                hearing.CaseType.Name, @case.Number);
        }

        public HearingDto Hearing { get; }
        public IntegrationEventType EventType { get; } = IntegrationEventType.HearingDetailsUpdated;
    }
}