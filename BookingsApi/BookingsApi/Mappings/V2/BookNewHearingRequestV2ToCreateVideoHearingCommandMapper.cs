using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V1;

namespace BookingsApi.Mappings.V2;

public static class BookNewHearingRequestV2ToCreateVideoHearingCommandMapper
{
    public static CreateVideoHearingCommand Map(
        BookNewHearingRequestV2 requestV2,
        CaseType caseType,
        HearingType hearingType,
        HearingVenue venue,
        List<Case> cases,
        IRandomGenerator randomGenerator,
        string sipAddressStem,
        List<HearingRole> hearingRoles)
    {
        var newParticipants = MapParticipants(requestV2, caseType, hearingRoles);
        var newEndpoints = MapEndpoints(requestV2, randomGenerator, sipAddressStem);
        var linkedParticipants = MapLinkedParticipants(requestV2);
        var judiciaryParticipants = MapJudiciaryParticipants(requestV2);

        return new CreateVideoHearingCommand(caseType, hearingType,
            requestV2.ScheduledDateTime, requestV2.ScheduledDuration, venue, newParticipants, cases,
            false, requestV2.AudioRecordingRequired, newEndpoints, linkedParticipants,
            requestV2.IsMultiDayHearing)
        {
            HearingRoomName = requestV2.HearingRoomName,
            OtherInformation = requestV2.OtherInformation,
            CreatedBy = requestV2.CreatedBy,
            JudiciaryParticipants = judiciaryParticipants
        };
    }

    private static List<NewJudiciaryParticipant> MapJudiciaryParticipants(BookNewHearingRequestV2 requestV2)
    {
        return requestV2.JudiciaryParticipants.Select(JudiciaryParticipantRequestToNewJudiciaryParticipantMapper.Map).ToList();
    }

    private static List<NewParticipant> MapParticipants(BookNewHearingRequestV2 requestV2, CaseType caseType, List<HearingRole> hearingRoles)
    {
        var newParticipants = requestV2.Participants.Select(
            x => ParticipantRequestV2ToNewParticipantMapper.Map(x, caseType, hearingRoles)).ToList();
        return newParticipants;
    }

    private static List<NewEndpoint> MapEndpoints(BookNewHearingRequestV2 requestV2, IRandomGenerator randomGenerator,
        string sipAddressStem)
    {
        var endpoints = new List<NewEndpoint>();
        if (requestV2.Endpoints != null)
        {
            endpoints = requestV2.Endpoints.Select(x =>
                EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(x, randomGenerator, sipAddressStem)).ToList();
        }

        return endpoints;
    }

    private static List<LinkedParticipantDto> MapLinkedParticipants(BookNewHearingRequestV2 requestV2)
    {
        var dto = LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(requestV2.LinkedParticipants)
            .ToList();
        return dto;
    }
}
