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
        /// The Judiciary person's title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        ///     Judiciary person's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Judiciary person's last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        ///     Judiciary person's full name.
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        ///     Judiciary person or participant's contact email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        ///     Judiciary person participant's work phone
        /// </summary>
        public string WorkPhone { get; set; }
        
        /// <summary>
        /// The participant's hearing role code
        /// </summary>
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; set; }

        
    }
}
