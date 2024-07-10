using System.ComponentModel.DataAnnotations;

namespace BookingsApi.Contract.V1.Requests
{
    public class EndpointRequest
    {
        [StringLength(255, ErrorMessage = "Display name max length is 255 characters")]
        [RegularExpression("^([-A-Za-z0-9 ',._])*$")]
        public string DisplayName { get; set; }
        public string DefenceAdvocateContactEmail { get; set; }
        
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