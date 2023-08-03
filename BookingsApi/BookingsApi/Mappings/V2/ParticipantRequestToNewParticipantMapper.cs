using System.Linq;
using BookingsApi.Common;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Mappings.V2
{
    /// <summary>
    /// This class is used to map a participant request object to the NewParticipant model
    /// used by the AddParticipantsToVideoHearingCommand.
    /// </summary>
    public static class ParticipantRequestToNewParticipantMapper
    {
        public static NewParticipant Map(ParticipantRequestV2 requestV2Participant, CaseType caseType)
        {
            var caseRole = caseType.CaseRoles.FirstOrDefault(x => x.Name == requestV2Participant.CaseRoleName);
            if (caseRole == null) throw new BadRequestException($"Invalid case role [{requestV2Participant.CaseRoleName}]");

            var hearingRole = caseRole.HearingRoles.FirstOrDefault(x => x.Name == requestV2Participant.HearingRoleName);
            if (hearingRole == null) throw new BadRequestException($"Invalid hearing role [{requestV2Participant.HearingRoleName}]");

            if (string.IsNullOrEmpty(requestV2Participant.Username))
            {
                requestV2Participant.Username = requestV2Participant.ContactEmail;
            }
            
            var person = new Person(requestV2Participant.Title, requestV2Participant.FirstName, requestV2Participant.LastName,
                requestV2Participant.ContactEmail, requestV2Participant.Username)
            {
                MiddleNames = requestV2Participant.MiddleNames,
                ContactEmail = requestV2Participant.ContactEmail,
                TelephoneNumber = requestV2Participant.TelephoneNumber
            };

            if(!string.IsNullOrEmpty(requestV2Participant.OrganisationName))
            {
                person.Organisation = new Organisation(requestV2Participant.OrganisationName);
            }

            return new NewParticipant
            {
                Person = person,
                CaseRole = caseRole,
                HearingRole = hearingRole,
                DisplayName = requestV2Participant.DisplayName,
                Representee = requestV2Participant.Representee
            };
        }
    }
}
