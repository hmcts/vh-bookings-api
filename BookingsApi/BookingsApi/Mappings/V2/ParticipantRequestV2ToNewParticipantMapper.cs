using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.Mappings.V2
{
    /// <summary>
    /// This class is used to map a participant request object to the NewParticipant model
    /// used by the AddParticipantsToVideoHearingCommand.
    /// </summary>
    public static class ParticipantRequestV2ToNewParticipantMapper
    {
        public static NewParticipant Map(ParticipantRequestV2 requestV2Participant, List<HearingRole> hearingRoles)
        {
            var hearingRole = hearingRoles.Find(x => string.Compare(x.Code, requestV2Participant.HearingRoleCode,
                StringComparison.InvariantCultureIgnoreCase) == 0);

            var person = new Person(requestV2Participant.Title, requestV2Participant.FirstName,
                requestV2Participant.LastName, requestV2Participant.ContactEmail)
            {
                MiddleNames = requestV2Participant.MiddleNames,
                ContactEmail = requestV2Participant.ContactEmail,
                TelephoneNumber = requestV2Participant.TelephoneNumber
            };

            if (!string.IsNullOrEmpty(requestV2Participant.OrganisationName))
            {
                person.Organisation = new Organisation(requestV2Participant.OrganisationName);
            }

            return new NewParticipant
            {
                Person = person,
                CaseRole = null,
                HearingRole = hearingRole,
                DisplayName = requestV2Participant.DisplayName,
                Representee = requestV2Participant.Representee
            };
        }
    }
}
