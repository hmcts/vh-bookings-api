using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.Mappings.V2
{
    public static class EndpointToResponseV2Mapper
    {
        public static EndpointResponseV2 MapEndpointToResponse(Endpoint endpoint)
        {
            return new EndpointResponseV2
            {
                Id = endpoint.Id,
                DisplayName = endpoint.DisplayName,
                Sip = endpoint.Sip,
                Pin = endpoint.Pin,
                DefenceAdvocateId = endpoint.GetDefenceAdvocate()?.Id,
            };
        }
        
        public static NewEndpointDto MapRequestToNewEndpointDto(EndpointRequestV2 requestV2, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            var sipComplete = sip + sipAddressStem;
            
            return new NewEndpointDto
            {
                Pin = pin,
                Sip = sipComplete,
                DisplayName = requestV2.DisplayName,
                EndpointParticipants = requestV2.EndpointParticipants?.Select(x => new NewEndpointParticipantDto
                {
                    ContactEmail = x.ContactEmail,
                    Type = (LinkedParticipantType)x.Type
                }).ToList() ?? new List<NewEndpointParticipantDto>()
            };
        }
    }
}