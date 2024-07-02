using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;

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
                ContactEmail = endpointRequest.DefenceAdvocateContactEmail,
                OtherLanguage = endpointRequest.OtherLanguage,
                LanguageCode = endpointRequest.InterpreterLanguageCode
            });
        }

        return endpoints;
    }
}

public class NewEndpointRequestDto
{
    public string DisplayName { get; set; }
    public string DefenceAdvocateContactEmail { get; set; }
    public string InterpreterLanguageCode { get; set; }
    public string OtherLanguage { get; set; }
}