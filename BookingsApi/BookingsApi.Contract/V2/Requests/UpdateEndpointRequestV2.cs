using System;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateEndpointRequestV2 : EndpointRequestV2
    {
        /// <summary>
        /// The id of the hearing
        /// </summary>
        public Guid Id { get; set; }
        
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
