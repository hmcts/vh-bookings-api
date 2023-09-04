using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
{
    public static class JudiciaryParticipantRequestToNewJudiciaryParticipantMapper
    {
        public static NewJudiciaryParticipant Map(JudiciaryParticipantRequest requestParticipant)
        {
            var hearingRoleCode = ApiJudiciaryParticipantHearingRoleCodeToDomainMapper.Map(requestParticipant.HearingRoleCode);

            return new NewJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = requestParticipant.PersonalCode,
                HearingRoleCode = hearingRoleCode
            };
        }
    }
}
