using System;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.DAL.Commands;

namespace BookingsApi.Mappings
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
                ContactEmail = request.DefenceAdvocateContactEmail
            };
        }
    }
}