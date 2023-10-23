using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2
{
    public class ParticipantToResponseV2Mapper
    {
        public ParticipantResponseV2 MapParticipantToResponse(Participant participant)
        {
            var participantResponse = new ParticipantResponseV2
            {
                Id = participant.Id,
                DisplayName = participant.DisplayName,
                HearingRoleCode = participant.HearingRole.Code,
                UserRoleName = participant.HearingRole.UserRole.Name,
                Title = participant.Person.Title,
                FirstName = participant.Person.FirstName,
                LastName = participant.Person.LastName,
                MiddleNames = participant.Person.MiddleNames,
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

            participantResponse.TrimAllStringsRecursively();
            return participantResponse;
        }
    }
}