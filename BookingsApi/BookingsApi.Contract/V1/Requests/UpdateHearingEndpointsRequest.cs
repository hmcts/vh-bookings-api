using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateHearingEndpointsRequest
    {
        /// <summary>
        ///     List of new endpoints
        /// </summary>
        public List<AddEndpointRequest> NewEndpoints { get; set; } = new();

        /// <summary>
        ///     List of existing endpoints
        /// </summary>
        public List<EditableEndpointRequest> ExistingEndpoints { get; set; } = new();

        /// <summary>
        ///     List of removed endpoint Ids
        /// </summary>
        public List<Guid> RemovedEndpointIds { get; set; } = new();
    }
}
