using Bookings.Api.Contract.Responses;
using Bookings.Domain.Participants;

namespace Bookings.API.Mappings
{
    public class ParticipantToResponseMapper
    {
        public ParticipantResponse MapParticipantToResponse(Participant participant)
        {
            return new ParticipantResponse
            {
                Id = participant.Id,
                DisplayName = participant.DisplayName,
                CaseRoleName = participant.CaseRole.Name,
                HearingRoleName = participant.HearingRole.Name,
                UserRoleName = participant.HearingRole.UserRole.Name,
                Title = participant.Person.Title,
                FirstName = participant.Person.FirstName,
                LastName = participant.Person.LastName,
                MiddleNames = participant.Person.MiddleNames,
                Username = participant.Person.Username,
                ContactEmail = participant.Person.ContactEmail,
                TelephoneNumber = participant.Person.TelephoneNumber,
                HouseNumber = participant.Person.Address?.HouseNumber,
                Street = participant.Person.Address?.Street,
                City = participant.Person.Address?.City,
                County = participant.Person.Address?.County,
                Postcode = participant.Person.Address?.Postcode
                
            };
        }
    }
}