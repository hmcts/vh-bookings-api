using System;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.Common.Services;
using Bookings.DAL.Commands;
using Bookings.Domain;
using Bookings.Domain.Participants;

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

        public static Endpoint MapRequestToEndpoint(EndpointRequest request, string sip, string pin,
            Participant defenceAdvocate)
        {
            return new Endpoint(request.DisplayName, sip, pin, defenceAdvocate);
        }

        public static NewEndpoint MapRequestToNewEndpointDto(EndpointRequest request, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            return new NewEndpoint
            {
                Pin = pin,
                Sip = $"{sip}{sipAddressStem}",
                DisplayName = request.DisplayName,
                DefenceAdvocateUsername = request.DefenceAdvocateUsername
            };
        }
    }
}