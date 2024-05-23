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

        public static NewEndpointDto MapRequestToNewEndpointDto(EndpointRequest request, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            var sipComplete = sip + sipAddressStem;
            var endpointParticipants = new List<NewEndpointParticipantDto>();
            if(!String.IsNullOrWhiteSpace(request.DefenceAdvocateContactEmail))
                endpointParticipants.Add(new NewEndpointParticipantDto { ContactEmail = request.DefenceAdvocateContactEmail, Type = LinkedParticipantType.DefenceAdvocate });
            return new NewEndpointDto
            {
                Pin = pin,
                Sip = sipComplete,
                DisplayName = request.DisplayName,
                EndpointParticipants = endpointParticipants
            };
        }
    }
}