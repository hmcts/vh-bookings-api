using System;

namespace Bookings.Api.Contract.Responses
{
    public class EndpointResponse
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
    }
}