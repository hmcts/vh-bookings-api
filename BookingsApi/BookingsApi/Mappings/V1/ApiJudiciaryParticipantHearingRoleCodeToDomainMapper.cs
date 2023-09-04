namespace BookingsApi.Mappings.V1
{
    public static class ApiJudiciaryParticipantHearingRoleCodeToDomainMapper
    {
        public static JudiciaryParticipantHearingRoleCode Map(Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode apiHearingRoleCode)
        {
            return apiHearingRoleCode switch
            {
                Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode.Judge => JudiciaryParticipantHearingRoleCode.Judge,
                Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode.PanelMember => JudiciaryParticipantHearingRoleCode.PanelMember,
                _ => throw new ArgumentOutOfRangeException(nameof(apiHearingRoleCode), apiHearingRoleCode, null)
            };
        }
    }
}
