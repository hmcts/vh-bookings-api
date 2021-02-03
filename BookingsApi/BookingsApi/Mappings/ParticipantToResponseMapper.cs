using BookingsApi.Contract.Responses;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Mappings
{
    public class ParticipantToResponseMapper
    {
        public ParticipantResponse MapParticipantToResponse(Participant participant)
        {
            var participantResponse = new ParticipantResponse
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
                Organisation = participant.Person.Organisation?.Name
            };

            switch (participant.HearingRole.UserRole.Name)
            {
                case "Representative":
                    var representative = (Representative)participant;
                    participantResponse.Representee = representative.Representee;
                    break;
                case "Individual":
                case "Judge":
                case "JudicialOfficeHolder":
                    break;
            }

            return participantResponse;
        }
    }
}