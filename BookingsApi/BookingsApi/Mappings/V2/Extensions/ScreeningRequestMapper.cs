using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.Mappings.V2.Extensions;

public static class ScreeningRequestMapper
{
    public static ScreeningDto MapToDalDto(this ScreeningRequest request)
    {
        return new ScreeningDto
        {
            ProtectFromEndpoints = request.ProtectFromEndpoints,
            ProtectFromParticipants = request.ProtectFromParticipants,
            ScreeningType = request.Type.MapToDomainEnum()
        };
    }
}