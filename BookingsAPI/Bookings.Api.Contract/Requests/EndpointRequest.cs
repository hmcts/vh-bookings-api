using System;

namespace Bookings.Api.Contract.Requests
{
    public class EndpointRequest
    {
        public string DisplayName { get; set; }
        public Guid? DefenceAdvocateId { get; set; }
    }
}