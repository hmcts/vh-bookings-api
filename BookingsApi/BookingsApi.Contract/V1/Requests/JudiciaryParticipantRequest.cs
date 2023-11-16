using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingsApi.Contract.V1.Requests
{
    public class JudiciaryParticipantRequest
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
        
        /// <summary>
        /// The contact telephone for the generic participant
        /// </summary>
        public string OptionalContactTelephone { get; set; }
        
        /// <summary>
        /// The contact email for the generic participant
        /// </summary>
        public string OptionalContactEmail { get; set; }
    }
}
