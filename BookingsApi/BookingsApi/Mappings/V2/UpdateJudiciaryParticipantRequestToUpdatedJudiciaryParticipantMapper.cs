using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.Mappings.V2
{
    public static class UpdateJudiciaryParticipantRequestToUpdatedJudiciaryParticipantMapper
    {
        public static UpdatedJudiciaryParticipant Map(string personalCode, UpdateJudiciaryParticipantRequest requestParticipant)
        {
            return new UpdatedJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = personalCode,
                HearingRoleCode = requestParticipant.HearingRoleCode.MapToDomainEnum(),
                InterpreterLanguageCode = requestParticipant.InterpreterLanguageCode,
                OtherLanguage = requestParticipant.OtherLanguage
            };
        }
    }
}
