using BookingsApi.Contract.V2.Requests.Enums;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateJudiciaryParticipantRequestV2
    {
        /// <summary>
        /// The participant's display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The participant's hearing role code
        /// </summary>
        public JudiciaryParticipantHearingRoleCodeV2 HearingRoleCode { get; set; }
    }
}
