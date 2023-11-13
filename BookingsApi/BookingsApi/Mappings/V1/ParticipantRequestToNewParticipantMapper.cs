using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
{
    /// <summary>
    /// This class is used to map a participant request object to the NewParticipant model
    /// used by the AddParticipantsToVideoHearingCommand.
    /// </summary>
    public static class ParticipantRequestToNewParticipantMapper
    {
        public static NewParticipant Map(ParticipantRequest requestParticipant, CaseType caseType)
        {
            var caseRole = caseType.CaseRoles.Find(x => x.Name.ToLowerInvariant() == requestParticipant.CaseRoleName.ToLowerInvariant());
            if (caseRole == null) throw new BadRequestException($"Invalid case role [{requestParticipant.CaseRoleName}]");

            var hearingRole = caseRole.HearingRoles.Find(x => x.Name.ToLowerInvariant() == requestParticipant.HearingRoleName.ToLowerInvariant());
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
