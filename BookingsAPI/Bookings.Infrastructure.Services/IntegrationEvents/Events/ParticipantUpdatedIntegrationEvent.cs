using System;
using Bookings.Domain.Participants;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantUpdatedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantUpdatedIntegrationEvent(Guid hearingId, Participant participant)
        {
            HearingId = hearingId;
            var representee = participant is Representative representative ? representative.Representee : string.Empty;

            Participant = new ParticipantDto(participant.Id,
                $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}",
                participant.Person.Username, participant.DisplayName, 
                participant.HearingRole.Name, participant.HearingRole.UserRole.Name, 
                participant.CaseRole.Group, representee);
        }

        public Guid HearingId { get; }
        public ParticipantDto Participant { get; }
    }
}