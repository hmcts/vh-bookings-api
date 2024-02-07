using System;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateEndpointRequestV2 : EndpointRequestV2
    {
        public Guid Id { get; set; }
    }
}
