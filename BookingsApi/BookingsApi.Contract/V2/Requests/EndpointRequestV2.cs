using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class EndpointRequestV2
    {
        public string DisplayName { get; set; }
        public List<EndpointParticipantsRequestV2> EndpointParticipants { get; set; }
    }
}