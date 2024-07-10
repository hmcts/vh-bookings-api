namespace BookingsApi.Contract.V1.Requests
{
    public class ReassignJudiciaryJudgeRequest
    {
        /// <summary>
        /// The participant's judicial personal code
        /// </summary>
        public string PersonalCode { get; set; }
        
        /// <summary>
        /// The participant's display name
        /// </summary>
        public string DisplayName { get; set; }
        public string OptionalContactEmail { get; set; }
        public string OptionalContactTelephone { get; set; }
        
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
