using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class EditableEndpointRequest : UpdateEndpointRequest
    {
        /// <summary>
        /// The id of the endpoint
        /// </summary>
        public Guid Id { get; set; }
    }
}
