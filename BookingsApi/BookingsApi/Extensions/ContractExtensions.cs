using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V2.Requests;
namespace BookingsApi.Extensions;

public static class ContractExtensions
{
    public static void SanitizeRequest(this BookNewHearingRequest request)
    {
        foreach (var participant in request.Participants)
        {
            participant.FirstName = participant.FirstName?.Trim();
            participant.LastName = participant.LastName?.Trim();
        }
    }
    
    public static void SanitizeRequest(this BookNewHearingRequestV2 request)
    {
        foreach (var participant in request.Participants)
        {
            participant.FirstName = participant.FirstName?.Trim();
            participant.LastName = participant.LastName?.Trim();
        }
    }
}