using System.Collections.Generic;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingIsReadyForVideoIntegrationEvent : IIntegrationEvent
    {
        public HearingIsReadyForVideoIntegrationEvent(Hearing hearing)
        {
            var @case = hearing.GetCases().First(); // Does this need to be a lead case?
            Hearing = new HearingDto(hearing.Id, hearing.ScheduledDateTime, hearing.ScheduledDuration,
                hearing.CaseType.Name, @case.Number, @case.Name, hearing.HearingVenueName, hearing.AudioRecordingRequired);

            var hearingParticipants = hearing.GetParticipants();

            Participants = hearingParticipants.Select(x =>
            {
                var representee = x is Representative representative ? representative.Representee : string.Empty;
                return new ParticipantDto(x.Id,
                    $"{x.Person.Title} {x.Person.FirstName} {x.Person.LastName}",
                    x.Person.Username, x.DisplayName, x.HearingRole.Name, x.HearingRole.UserRole.Name,
                    x.CaseRole.Group, representee);
            }).ToList();
        }

        public HearingDto Hearing { get; }
        public IList<ParticipantDto> Participants { get; }
    }
}

    