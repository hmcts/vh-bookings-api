using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class ParticipantDtoMapper
    {
        public static ParticipantDto MapToDto(Participant participant)
        {
            var representee = participant is Representative representative ? representative.Representee : string.Empty;
            return
                new ParticipantDto
                {
                    ParticipantId = participant.Id,
                    Fullname = $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}",
                    Username = participant.Person.Username,
                    FirstName = participant.Person.FirstName,
                    LastName = participant.Person.LastName,
                    ContactEmail = participant.Person.ContactEmail,
                    ContactTelephone = participant.Person.TelephoneNumber,
                    DisplayName = participant.DisplayName,
                    HearingRole = participant.HearingRole.Name,
                    UserRole = participant.HearingRole.UserRole.Name,
                    CaseGroupType = participant.CaseRole.Group,
                    Representee = representee,
                    LinkedParticipants = participant.LinkedParticipants
                };
        }
    }
}