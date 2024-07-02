using System;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateEndpointRequestV2 : EndpointRequestV2
    {
        /// <summary>
        /// The id of the hearing
        /// </summary>
        public Guid Id { get; set; }
    }
}
