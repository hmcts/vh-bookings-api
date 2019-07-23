using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Participants;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantsAddedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantsAddedIntegrationEvent(Guid hearingId, IEnumerable<Participant> participants)
        {
            Participants = new List<ParticipantDto>();
            HearingId = hearingId;
            participants.ToList().ForEach(AddParticipant);
        }

        public Guid HearingId { get; }

        public IList<ParticipantDto> Participants { get; }

        private void AddParticipant(Participant participant)
        {
            var representee = participant is Representative representative ? representative.Representee : string.Empty;

            var participantDto = new ParticipantDto(participant.Id,
                $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}",
                participant.Person.Username, participant.DisplayName,
                participant.HearingRole.Name, participant.HearingRole.UserRole.Name,
                participant.CaseRole.Group, representee);

            Participants.Add(participantDto);
        }
    }
}