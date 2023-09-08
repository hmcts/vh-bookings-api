using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingsApi.Contract.V1.Responses
{
    public class JudiciaryParticipantResponse
    {
        /// <summary>
        /// The participant's judicial personal code
        /// </summary>
        public string PersonalCode { get; set; }
        
        /// <summary>
        /// The participant's display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The participant's hearing role code
        /// </summary>
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; set; }
    }
}
