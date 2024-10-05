using BookingsApi.DAL.Dtos;
using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.DAL.Extensions;

public static class ScreeningExtensions
{
    public static ScreeningDto MapToScreeningDto(this Screening screening)
    {
        if (screening == null)
        {
            return null;
        }

        var endpointIds = screening.GetEndpoints().Select(x => x.Endpoint.ExternalReferenceId).ToList();
        var participantIds = screening.GetParticipants().Select(x => x.Participant.ExternalReferenceId).ToList();
        
        return new ScreeningDto
        {
            ScreeningType = screening.Type,
            ProtectedFrom = endpointIds.Concat(participantIds).ToList()
        };
    }
}