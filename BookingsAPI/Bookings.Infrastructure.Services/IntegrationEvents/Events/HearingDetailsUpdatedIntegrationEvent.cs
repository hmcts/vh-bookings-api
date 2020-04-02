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
            Hearing = new HearingDto(hearing.Id, hearing.ScheduledDateTime, hearing.ScheduledDuration,
                hearing.CaseType.Name, @case.Number, @case.Name, hearing.HearingVenueName)
            {
                RecordAudio = hearing.AudioRecordingRequired
            };
        }

        public HearingDto Hearing { get; }
    }
}