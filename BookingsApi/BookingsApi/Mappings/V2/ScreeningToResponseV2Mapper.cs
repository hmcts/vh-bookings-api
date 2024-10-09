using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2;

public static class ScreeningToResponseV2Mapper
{
    public static ScreeningResponseV2 MapScreeningToResponse(Screening screening)
    {
        return new ScreeningResponseV2
        {
            Type = screening.Type.MapToContractEnum(),
            ProtectFromEndpointsIds = screening.GetEndpoints().Select(x=> x.EndpointId!.Value).ToList(),
            ProtectFromParticipantsIds = screening.GetParticipants().Select(x=> x.ParticipantId!.Value).ToList()
        };
    }
}