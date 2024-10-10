using System;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Contract.V2.Responses
{
    public class EndpointResponseV2
    {
        public Guid Id { get; set; }
        /// <summary>
        /// The external reference id
        /// </summary>
        public string ExternalReferenceId { get; set; }
        
        /// <summary>
        /// An external identifier for the special measure for the participant (optional)
        /// </summary>
        public string MeasuresExternalId { get; set; }
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public Guid? DefenceAdvocateId { get; set; }
        public InterpreterLanguagesResponse InterpreterLanguage { get; set; }
        public string OtherLanguage { get; set; }
        
        /// <summary>
        ///   The endpoint's screening needs (optional)
        /// </summary>
        public ScreeningResponseV2 Screening { get; set; }
    }
}