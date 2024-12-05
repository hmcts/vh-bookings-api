using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.Mappings.V2
{
    public static class JudiciaryParticipantRequestToNewJudiciaryParticipantMapper
    {
        public static NewJudiciaryParticipant Map(JudiciaryParticipantRequest requestParticipant)
        {
            return new NewJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = requestParticipant.PersonalCode,
                HearingRoleCode = requestParticipant.HearingRoleCode.MapToDomainEnum(),
                OptionalContactEmail = requestParticipant.ContactEmail,
                OptionalContactTelephone = requestParticipant.ContactTelephone,
                InterpreterLanguageCode = requestParticipant.InterpreterLanguageCode,
                OtherLanguage = requestParticipant.OtherLanguage
            };
        }
    }
}
