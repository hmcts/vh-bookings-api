using Bookings.Domain.Participants;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services
{
    public static class ParticipantDtoMapper
    {
        public static ParticipantDto MapToDto(Participant participant)
        {
            var representee = participant is Representative representative ? representative.Representee : string.Empty;
            return new ParticipantDto(participant.Id,
                $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}",
                participant.Person.Username, participant.DisplayName, participant.HearingRole.Name, participant.HearingRole.UserRole.Name,
                participant.CaseRole.Group, representee);
        }
    }
}