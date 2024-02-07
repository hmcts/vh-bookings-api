using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateHearingEndpointsRequestV2
    {
        /// <summary>
        ///     List of new endpoints
        /// </summary>
        public List<EndpointRequestV2> NewEndpoints { get; set; } = new();

        /// <summary>
        ///     List of existing endpoints
        /// </summary>
        public List<UpdateEndpointRequestV2> ExistingEndpoints { get; set; } = new();

        /// <summary>
        ///     List of removed endpoint Ids
        /// </summary>
        public List<Guid> RemovedEndpointIds { get; set; } = new();
    }
}
