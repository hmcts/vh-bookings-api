using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2.Extensions;

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

            // For new user we don't have the username yet.
            // We need to set the username to contact email temporarily.
            // This will be changed and updated after creating the user.
            var person = new Person(requestV2Participant.Title, requestV2Participant.FirstName,
                requestV2Participant.LastName, requestV2Participant.ContactEmail, requestV2Participant.ContactEmail)
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
                HearingRole = hearingRole,
                DisplayName = requestV2Participant.DisplayName,
                Representee = requestV2Participant.Representee,
                OtherLanguage = requestV2Participant.OtherLanguage,
                InterpreterLanguageCode = requestV2Participant.InterpreterLanguageCode,
                Screening = requestV2Participant.Screening?.MapToDalDto(),
                ExternalReferenceId = requestV2Participant.ExternalParticipantId,
                MeasuresExternalId = requestV2Participant.MeasuresExternalId,
            };
        }
    }
}
