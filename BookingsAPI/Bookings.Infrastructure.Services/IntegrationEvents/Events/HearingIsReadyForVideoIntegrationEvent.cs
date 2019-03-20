using System.Collections.Generic;
using System.Linq;
using Bookings.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingIsReadyForVideoIntegrationEvent : IIntegrationEvent
    {
        public HearingIsReadyForVideoIntegrationEvent(Hearing hearing)
        {
            var @case = hearing.GetCases().First(); // is this needs to ba lead case?
            Hearing = new HearingDto(hearing.Id, hearing.ScheduledDateTime, hearing.ScheduledDuration,
                hearing.CaseType.Name, @case.Number);

            Participants = hearing.GetParticipants().Select(x => new ParticipantDto(x.Id,
                $"{x.Person.Title} {x.Person.FirstName} {x.Person.LastName}",
                x.Person.Username, x.DisplayName, x.HearingRole.UserRole.Name, x.CaseRole.Group)).ToList();
        }

        public HearingDto Hearing { get; }
        public IList<ParticipantDto> Participants { get; }
        public IntegrationEventType EventType { get; } = IntegrationEventType.HearingIsReadyForVideo;
    }
}

    