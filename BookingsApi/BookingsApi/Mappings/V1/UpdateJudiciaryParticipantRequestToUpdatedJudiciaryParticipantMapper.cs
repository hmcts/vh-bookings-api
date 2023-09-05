using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
{
    public static class UpdateJudiciaryParticipantRequestToUpdatedJudiciaryParticipantMapper
    {
        public static UpdatedJudiciaryParticipant Map(string personalCode, UpdateJudiciaryParticipantRequest requestParticipant)
        {
            var hearingRoleCode = ApiJudiciaryParticipantHearingRoleCodeToDomainMapper.Map(requestParticipant.HearingRoleCode);

            return new UpdatedJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = personalCode,
                HearingRoleCode = hearingRoleCode
            };
        }
    }
}
