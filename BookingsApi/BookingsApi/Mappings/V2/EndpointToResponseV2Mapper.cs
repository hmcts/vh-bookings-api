using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V1;

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
                DefenceAdvocateId = endpoint.DefenceAdvocate?.Id,
                InterpreterLanguage = endpoint.InterpreterLanguage != null ?
                    InterpreterLanguageToResponseMapper.MapInterpreterLanguageToResponse(endpoint.InterpreterLanguage) :
                    null,
                OtherLanguage = endpoint.OtherLanguage
            };
        }

        public static NewEndpoint MapRequestToNewEndpointDto(EndpointRequestV2 requestV2, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            var sipComplete = sip + sipAddressStem;
            return new NewEndpoint
            {
                Pin = pin,
                Sip = sipComplete,
                DisplayName = requestV2.DisplayName,
                ContactEmail = requestV2.DefenceAdvocateContactEmail,
                OtherLanguage = requestV2.OtherLanguage,
                LanguageCode = requestV2.InterpreterLanguageCode,
                Screening = requestV2.Screening == null ? null : new ScreeningDto
                {
                    ProtectFromEndpoints = requestV2.Screening.ProtectFromParticipants,
                    ProtectFromParticipants = requestV2.Screening.ProtectFromParticipants,
                    ScreeningType = (ScreeningType)requestV2.Screening.Type
                }
            };
        }
    }
}