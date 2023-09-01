using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
{
    public static class JudiciaryParticipantRequestToNewJudiciaryParticipantMapper
    {
        public static NewJudiciaryParticipant Map(JudiciaryParticipantRequest requestParticipant)
        {
            var hearingRoleCode = requestParticipant.HearingRoleCode switch
            {
                Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode.Judge => JudiciaryParticipantHearingRoleCode.Judge,
                Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode.PanelMember => JudiciaryParticipantHearingRoleCode.PanelMember,
                _ => throw new ArgumentOutOfRangeException(nameof(requestParticipant), requestParticipant.HearingRoleCode, null)
            };

            return new NewJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = requestParticipant.PersonalCode,
                HearingRoleCode = hearingRoleCode
            };
        }
    }
}
