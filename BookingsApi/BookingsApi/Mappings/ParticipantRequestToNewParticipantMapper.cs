using System.Linq;
using BookingsApi.Contract.Requests;
using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Commands.V1;

namespace BookingsApi.Mappings
{
    /// <summary>
    /// This class is used to map a participant request object to the NewParticipant model
    /// used by the AddParticipantsToVideoHearingCommand.
    /// </summary>
    public static class ParticipantRequestToNewParticipantMapper
    {
        public static NewParticipant Map(ParticipantRequest requestParticipant, CaseType caseType)
        {
            var caseRole = caseType.CaseRoles.FirstOrDefault(x => x.Name == requestParticipant.CaseRoleName);
            if (caseRole == null) throw new BadRequestException($"Invalid case role [{requestParticipant.CaseRoleName}]");

            var hearingRole = caseRole.HearingRoles.FirstOrDefault(x => x.Name == requestParticipant.HearingRoleName);
            if (hearingRole == null) throw new BadRequestException($"Invalid hearing role [{requestParticipant.HearingRoleName}]");

            if (string.IsNullOrEmpty(requestParticipant.Username))
            {
                requestParticipant.Username = requestParticipant.ContactEmail;
            }
            
            var person = new Person(requestParticipant.Title, requestParticipant.FirstName, requestParticipant.LastName,
                requestParticipant.ContactEmail, requestParticipant.Username)
            {
                MiddleNames = requestParticipant.MiddleNames,
                ContactEmail = requestParticipant.ContactEmail,
                TelephoneNumber = requestParticipant.TelephoneNumber
            };

            if(!string.IsNullOrEmpty(requestParticipant.OrganisationName))
            {
                person.Organisation = new Organisation(requestParticipant.OrganisationName);
            }

            return new NewParticipant
            {
                Person = person,
                CaseRole = caseRole,
                HearingRole = hearingRole,
                DisplayName = requestParticipant.DisplayName,
                Representee = requestParticipant.Representee
            };
        }
    }
}
