using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class EndpointRequestV2
    {
        /// <summary>
        /// The endpoint display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The contact email of the defence advocated linked to the endpoint. Deprecated, use LinkedParticipants list instead
        /// </summary>
        [Obsolete("Use LinkedParticipants list instead")]
        public string DefenceAdvocateContactEmail { get; set; }
        
        /// <summary>
        /// Participants linked to the endpoint
        /// </summary>
        public List<string> LinkedParticipantEmails { get; set; } = new ();
        
        /// <summary>
        ///     The code of the interpreter language (optional)
        /// </summary>
        public string InterpreterLanguageCode { get; set; }
        
        /// <summary>
        ///     Interpreter language, specify this when the interpreter language code is not available (optional)
        /// </summary>
        public string OtherLanguage { get; set; }
        
        /// <summary>
        /// A unique identifier for the participant from an external system
        /// </summary>
        public string ExternalParticipantId { get; set; }
        
        /// <summary>
        /// An external identifier for the special measure for the participant (optional)
        /// </summary>
        public string MeasuresExternalId { get; set; }
        
        /// <summary>
        ///    Screening requirements (special measure) for the endpoint (optional)
        /// </summary>
        public ScreeningRequest Screening { get; set; }
    }
}