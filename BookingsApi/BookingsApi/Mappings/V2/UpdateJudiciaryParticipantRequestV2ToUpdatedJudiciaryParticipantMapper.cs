using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.Mappings.V2
{
    public static class UpdateJudiciaryParticipantRequestV2ToUpdatedJudiciaryParticipantMapper
    {
        public static UpdatedJudiciaryParticipant Map(string personalCode, UpdateJudiciaryParticipantRequestV2 requestParticipant)
        {
            return new UpdatedJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = personalCode,
                HearingRoleCode = requestParticipant.HearingRoleCode.MapToDomainEnum()
            };
        }
    }
}
