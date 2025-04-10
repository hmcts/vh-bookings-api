using System.ComponentModel.DataAnnotations;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Requests
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
        [StringLength(255, ErrorMessage = "Display name max length is 255 characters")]
        [RegularExpression(@"^[\p{L}\p{N}\s',._-]+$")]
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The participant's hearing role code
        /// </summary>
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; set; }
        
        /// <summary>
        /// Optional Contact Email
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        /// Optional Contact Telephone
        /// </summary>
        public string ContactTelephone { get; set; }
        
        /// <summary>
        /// The code of the interpreter language
        /// </summary>
        public string InterpreterLanguageCode { get; set; }
        
        /// <summary>
        /// Interpreter language, specify this when the interpreter language code is not available
        /// </summary>
        public string OtherLanguage { get; set; }
    }
}
