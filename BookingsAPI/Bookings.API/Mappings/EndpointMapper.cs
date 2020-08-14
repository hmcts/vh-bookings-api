using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    public static class EndpointMapper
    {
        public static EndpointResponse MapEndpointToResponse(Endpoint endpoint)
        {
            return new EndpointResponse
            {
                Id = endpoint.Id,
                DisplayName = endpoint.DisplayName,
                Sip = endpoint.Sip,
                Pin = endpoint.Pin
            };
        }

        public static Endpoint MapRequestToEndpoint(EndpointRequest request, string sip, string pin)
        {
            return new Endpoint(request.DisplayName, sip, pin);
        }
    }
}