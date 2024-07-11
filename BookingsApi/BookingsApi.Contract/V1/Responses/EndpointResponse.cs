using System;

namespace BookingsApi.Contract.V1.Responses
{
    public class EndpointResponse
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public Guid? DefenceAdvocateId { get; set; }
        
        /// <summary>
        /// The code of the interpreter language
        /// </summary>
        public string InterpreterLanguageCode { get; set; }
        
        /// <summary>
        /// Interpreter language, for when the interpreter language code is not available
        /// </summary>
        public string OtherLanguage { get; set; }
    }
}