using System;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.Common.Services;
using Bookings.DAL.Commands;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    public static class EndpointToResponseMapper
    {
        public static EndpointResponse MapEndpointToResponse(Endpoint endpoint)
        {
            return new EndpointResponse
            {
                Id = endpoint.Id,
                DisplayName = endpoint.DisplayName,
                Sip = endpoint.Sip,
                Pin = endpoint.Pin,
                DefenceAdvocateId = endpoint.DefenceAdvocate?.Id
            };
        }

        public static NewEndpoint MapRequestToNewEndpointDto(EndpointRequest request, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            var sipComplete = sip + sipAddressStem;
            return new NewEndpoint
            {
                Pin = pin,
                Sip = sipComplete,
                DisplayName = request.DisplayName,
                DefenceAdvocateUsername = request.DefenceAdvocateUsername
            };
        }
    }
}