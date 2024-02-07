using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class EditableEndpointRequest : UpdateEndpointRequest
    {
        public Guid Id { get; set; }
    }
}
