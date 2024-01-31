using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class EditableEndpointRequest : EndpointRequest
    {
        /// <summary>
        ///     Id of the participant, set to null if new endpoint
        /// </summary>
        public Guid? Id { get; set; }
    }
}
