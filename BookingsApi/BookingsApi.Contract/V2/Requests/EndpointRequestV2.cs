namespace BookingsApi.Contract.V2.Requests
{
    public class EndpointRequestV2
    {
        /// <summary>
        /// The endpoint display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The contact email of the defence advocated linked to the endpoint
        /// </summary>
        public string DefenceAdvocateContactEmail { get; set; }
        
        /// <summary>
        ///     The code of the interpreter language
        /// </summary>
        public string InterpreterLanguageCode { get; set; }
        
        /// <summary>
        ///     Interpreter language, specify this when the interpreter language code is not available
        /// </summary>
        public string OtherLanguage { get; set; }
    }
}