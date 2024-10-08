using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
{
    [Obsolete("Use V2 mappers instead")]
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
                DefenceAdvocateId = endpoint.DefenceAdvocate?.Id,
                InterpreterLanguageCode = endpoint.InterpreterLanguage?.Code,
                OtherLanguage = endpoint.OtherLanguage
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
                ContactEmail = request.DefenceAdvocateContactEmail,
                LanguageCode = request.InterpreterLanguageCode,
                OtherLanguage = request.OtherLanguage
            };
        }
    }
}