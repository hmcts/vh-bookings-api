using System.Linq;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.Participants;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2
{
    public class ParticipantToResponseMapper
    {
        public ParticipantResponseV2 MapParticipantToResponse(Participant participant)
        {
            var participantResponse = new ParticipantResponseV2
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
                Organisation = participant.Person.Organisation?.Name,
                LinkedParticipants = participant.LinkedParticipants.Select(x => new LinkedParticipantResponseV2
                    {LinkedId = x.LinkedId, TypeV2 = x.Type.MapToContractEnum()}).ToList()
            };

            switch (participant.HearingRole.UserRole.Name)
            {
                case "Representative":
                    var representative = (Representative)participant;
                    participantResponse.Representee = representative.Representee;
                    break;
                case "Individual":
                case "Judge":
                case "StaffMember":
                case "JudicialOfficeHolder":
                    break;
            }

            return participantResponse;
        }
    }
}