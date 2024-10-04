using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2;

public static class ScreeningToResponseV2Mapper
{
    public static ScreeningResponseV2 MapScreeningToResponse(Screening screening)
    {
        var endpointIds = screening.GetEndpoints().Select(x => x.Endpoint.ExternalReferenceId).ToList();
        var participantIds = screening.GetParticipants().Select(x => x.Participant.ExternalReferenceId).ToList();
        
        return new ScreeningResponseV2
        {
            Type = screening.Type.MapToContractEnum(),
            ProtectedFrom = endpointIds.Concat(participantIds).ToList()
        };
    }
}