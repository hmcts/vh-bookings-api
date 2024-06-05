using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Responses
{
    public class EndpointResponseV2
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        
        public List<EndpointParticipantResponse> EndpointParticipants { get; set; }
    }
}