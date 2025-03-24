using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;

namespace BookingsApi.DAL.Helper;

/// <summary>
/// Safely generate new endpoints for a hearing.
/// The weak deterministic operation can cause issues on hardware that runs the loop too quickly.
/// </summary>
public static class NewEndpointGenerator
{
    public static List<NewEndpoint> GenerateNewEndpoints(List<NewEndpointRequestDto> endpointRequests,
        IRandomGenerator randomGenerator, string sipAddressStem)
    {
        var endpoints = new List<NewEndpoint>();

        foreach (var endpointRequest in endpointRequests)
        {
            string sip;

            do
            {
                sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            } while (endpoints.Exists(x => x.Sip.StartsWith(sip)));

            var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            endpoints.Add(new NewEndpoint
            {
                Pin = pin,
                Sip = $"{sip}{sipAddressStem}",
                DisplayName = endpointRequest.DisplayName,
                LinkedParticipantEmails = endpointRequest.LinkedParticipantEmails,
                OtherLanguage = endpointRequest.OtherLanguage,
                LanguageCode = endpointRequest.InterpreterLanguageCode,
                Screening = endpointRequest.Screening,
                MeasuresExternalId = endpointRequest.MeasuresExternalId,
                ExternalParticipantId = endpointRequest.ExternalReferenceId
            });
        }

        return endpoints;
    }
}

public class NewEndpointRequestDto
{
    public string ExternalReferenceId { get; set; }
    public string MeasuresExternalId { get; set; }
    public string DisplayName { get; set; }
    public List<string> LinkedParticipantEmails { get; set; }
    public string InterpreterLanguageCode { get; set; }
    public string OtherLanguage { get; set; }
    public ScreeningDto Screening { get; set; }
}