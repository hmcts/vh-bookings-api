using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.Mappings.V1
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
                DefenceAdvocateId = endpoint.GetDefenceAdvocate()?.Id,
            };
        }

        public static EndpointDto MapRequestToNewEndpointDto(EndpointRequest request, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            var sipComplete = sip + sipAddressStem;
            return new EndpointDto
            {
                Pin = pin,
                Sip = sipComplete,
                DisplayName = request.DisplayName,
                EndpointParticipants = new List<EndpointParticipantDto>
                {
                    new() { ContactEmail = request.DefenceAdvocateContactEmail, Type = LinkedParticipantType.DefenceAdvocate  }
                }
            };
        }
    }
}