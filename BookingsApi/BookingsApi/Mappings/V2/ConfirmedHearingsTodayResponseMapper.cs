
using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Mappings.V2;

public static class ConfirmedHearingsTodayResponseMapperV2
{
    public static ConfirmedHearingsTodayResponseV2 Map(Hearing videoHearing)
    {
        var participantMapper = new ParticipantToResponseV2Mapper();
            
        var participants = videoHearing.GetParticipants()
            .Select(x => participantMapper.MapParticipantToResponse(x))
            .ToList();

        var endpoints = videoHearing.GetEndpoints()
            .Select(EndpointToResponseV2Mapper.MapEndpointToResponse)
            .ToList();

        return new ConfirmedHearingsTodayResponseV2
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