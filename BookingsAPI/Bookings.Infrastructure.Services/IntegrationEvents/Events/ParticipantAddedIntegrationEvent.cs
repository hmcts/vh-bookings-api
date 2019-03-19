using System;
using Bookings.Domain.Participants;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantAddedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantAddedIntegrationEvent(Guid hearingId, Participant participant)
        {
            HearingId = hearingId;
            Participant = new ParticipantDto(participant.Id,
                $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}",
                participant.Person.Username, participant.DisplayName, participant.HearingRole.UserRole.Name, participant.CaseRole.Group);
        }

        public Guid HearingId { get; }
        public ParticipantDto Participant { get; }
        public IntegrationEventType EventType { get; } = IntegrationEventType.ParticipantAdded;
    }
}