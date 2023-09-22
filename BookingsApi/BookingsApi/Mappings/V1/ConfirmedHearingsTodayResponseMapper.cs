using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1;

public static class ConfirmedHearingsTodayResponseMapper
{
    public static ConfirmedHearingsTodayResponse Map(Hearing videoHearing)
    {
        var participantMapper = new ParticipantToResponseMapper();
            
        var participants = videoHearing.GetParticipants()
            .Select(x => participantMapper.MapParticipantToResponse(x))
            .ToList();

        var endpoints = videoHearing.GetEndpoints()
            .Select(EndpointToResponseMapper.MapEndpointToResponse)
            .ToList();

        return new ConfirmedHearingsTodayResponse
        {
            Id = videoHearing.Id,
            ScheduledDuration = videoHearing.ScheduledDuration,
            ScheduledDateTime = videoHearing.ScheduledDateTime,
            CaseTypeName = videoHearing.CaseType.Name,
            CaseName = videoHearing.GetCases()[0].Name,
            CaseNumber = videoHearing.GetCases()[0].Number,
            Participants = participants,
            Endpoints = endpoints,
            IsHearingVenueScottish = videoHearing.HearingVenue.IsScottish
        };
    }
}