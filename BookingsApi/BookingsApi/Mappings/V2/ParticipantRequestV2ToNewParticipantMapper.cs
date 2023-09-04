using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.Mappings.V2
{
    /// <summary>
    /// This class is used to map a participant request object to the NewParticipant model
    /// used by the AddParticipantsToVideoHearingCommand.
    /// </summary>
    public static class ParticipantRequestV2ToNewParticipantMapper
    {
        public static NewParticipant Map(ParticipantRequestV2 requestV2Participant, CaseType caseType, List<HearingRole> hearingRoles)
        {
            HearingRole hearingRole;
            CaseRole caseRole = null;
            // if no case role is provided, this request is using the flat structure
            if (string.IsNullOrEmpty(requestV2Participant.CaseRoleName))
            {
                hearingRole = hearingRoles.Find(x => x.Name == requestV2Participant.HearingRoleName);
            }
            else
            {
                caseRole = caseType.CaseRoles.Find(x => x.Name == requestV2Participant.CaseRoleName);
                hearingRole = caseRole.HearingRoles.Find(x => x.Name == requestV2Participant.HearingRoleName);
            }

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
