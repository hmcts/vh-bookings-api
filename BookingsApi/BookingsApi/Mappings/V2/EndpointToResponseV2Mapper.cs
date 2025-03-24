using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2
{
    /// <summary>
    /// Operations to map between request/response objects and domains for endpoints
    /// </summary>
    public static class EndpointToResponseV2Mapper
    {
        /// <summary>
        /// Map a JVS endpoint to an external facing Endpoint response model
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static EndpointResponseV2 MapEndpointToResponse(Endpoint endpoint)
        {
            var screeningResponse = endpoint.Screening == null
                ? null
                : ScreeningToResponseV2Mapper.MapScreeningToResponse(endpoint.Screening);
            
            return new EndpointResponseV2
            {
                Id = endpoint.Id,
                ExternalReferenceId = endpoint.ExternalReferenceId,
                MeasuresExternalId = endpoint.MeasuresExternalId,
                DisplayName = endpoint.DisplayName,
                Sip = endpoint.Sip,
                Pin = endpoint.Pin,
                DefenceAdvocateId = endpoint.ParticipantsLinked?.FirstOrDefault()?.Id,
                LinkedParticipantIds = endpoint.ParticipantsLinked?
                    .Select(pl => pl.Id)
                    .ToList(),
                InterpreterLanguage = endpoint.InterpreterLanguage != null ?
                    InterpreterLanguageToResponseMapperV2.MapInterpreterLanguageToResponse(endpoint.InterpreterLanguage) :
                    null,
                OtherLanguage = endpoint.OtherLanguage,
                Screening = screeningResponse
            };
        }

        /// <summary>
        /// Create a DTO for a new endpoint from a request
        /// </summary>
        /// <param name="requestV2"></param>
        /// <param name="randomGenerator"></param>
        /// <param name="sipAddressStem"></param>
        /// <returns></returns>
        public static NewEndpoint MapRequestToNewEndpointDto(EndpointRequestV2 requestV2, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            var sipComplete = sip + sipAddressStem;
            var linkedParticipantEmails = requestV2.LinkedParticipantEmails;
            linkedParticipantEmails.Add(requestV2.DefenceAdvocateContactEmail);
            return new NewEndpoint
            {
                Pin = pin,
                Sip = sipComplete,
                DisplayName = requestV2.DisplayName,
                LinkedParticipantEmails = linkedParticipantEmails,
                OtherLanguage = requestV2.OtherLanguage,
                LanguageCode = requestV2.InterpreterLanguageCode,
                ExternalParticipantId = requestV2.ExternalParticipantId,
                MeasuresExternalId = requestV2.MeasuresExternalId,
                Screening = requestV2.Screening?.MapToDalDto()
            };
        }
    }
}