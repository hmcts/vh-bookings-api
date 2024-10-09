using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.Common;

namespace BookingsApi.Mappings.V2;

internal static class ConfirmedHearingsTodayResponseMapperV2
{
    internal static ConfirmedHearingsTodayResponseV2 Map(Hearing videoHearing)
    {
        var participantMapper = new ParticipantToResponseV2Mapper();
        var judiciaryMapper = new JudiciaryParticipantToResponseMapper();
            
        var participants = videoHearing.GetParticipants()
            .Select(x => participantMapper.MapParticipantToResponse(x))
            .ToList();

        var judiciary = videoHearing.GetJudiciaryParticipants()
            .Select(x => judiciaryMapper.MapJudiciaryParticipantToResponse(x))
            .ToList();

        var endpoints = videoHearing.GetEndpoints()
            .Select(EndpointToResponseV2Mapper.MapEndpointToResponse)
            .ToList();

        return new ConfirmedHearingsTodayResponseV2
        {
            Id = videoHearing.Id,
            ScheduledDuration = videoHearing.ScheduledDuration,
            ScheduledDateTime = videoHearing.ScheduledDateTime,
            ServiceId = videoHearing.CaseType.ServiceId,
            ServiceName = videoHearing.CaseType.Name,
            CaseName = videoHearing.GetCases()[0].Name,
            CaseNumber = videoHearing.GetCases()[0].Number,
            Participants = participants,
            JudicaryParticipants = judiciary,
            Endpoints = endpoints,
            IsHearingVenueScottish = videoHearing.HearingVenue.IsScottish
        };
    }
}