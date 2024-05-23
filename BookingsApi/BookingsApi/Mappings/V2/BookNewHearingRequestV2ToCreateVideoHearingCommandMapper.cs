using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Dtos;
using BookingsApi.Mappings.V1;

namespace BookingsApi.Mappings.V2;

public static class BookNewHearingRequestV2ToCreateVideoHearingCommandMapper
{
    public static CreateVideoHearingCommand Map(
        BookNewHearingRequestV2 requestV2,
        CaseType caseType,
        HearingVenue venue,
        List<Case> cases,
        IRandomGenerator randomGenerator,
        string sipAddressStem,
        List<HearingRole> hearingRoles)
    {
        var newParticipants = MapParticipants(requestV2, hearingRoles);
        var newEndpoints = MapEndpoints(requestV2, randomGenerator, sipAddressStem);
        var linkedParticipants = MapLinkedParticipants(requestV2);
        var judiciaryParticipants = MapJudiciaryParticipants(requestV2);

        return new CreateVideoHearingCommand(
            new CreateVideoHearingRequiredDto(caseType, requestV2.ScheduledDateTime,
                requestV2.ScheduledDuration, venue, cases),
            new CreateVideoHearingOptionalDto(newParticipants, requestV2.HearingRoomName, requestV2.OtherInformation,
                requestV2.CreatedBy, requestV2.AudioRecordingRequired, newEndpoints, null, linkedParticipants,
                judiciaryParticipants, requestV2.IsMultiDayHearing, null, HearingType:null)
        );
    }

    private static List<NewJudiciaryParticipant> MapJudiciaryParticipants(BookNewHearingRequestV2 requestV2)
    {
        return requestV2.JudiciaryParticipants.Select(JudiciaryParticipantRequestToNewJudiciaryParticipantMapper.Map).ToList();
    }

    private static List<NewParticipant> MapParticipants(BookNewHearingRequestV2 requestV2, List<HearingRole> hearingRoles)
    {
        var newParticipants = requestV2.Participants.Select(
            x => ParticipantRequestV2ToNewParticipantMapper.Map(x, hearingRoles)).ToList();
        return newParticipants;
    }

    private static List<NewEndpointDto> MapEndpoints(BookNewHearingRequestV2 requestV2, IRandomGenerator randomGenerator, string sipAddressStem)
    {
        var endpoints = new List<NewEndpointDto>();
        if (requestV2.Endpoints != null)
            endpoints = requestV2.Endpoints.Select(x => EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(x, randomGenerator, sipAddressStem)).ToList();

        return endpoints;
    }

    private static List<LinkedParticipantDto> MapLinkedParticipants(BookNewHearingRequestV2 requestV2)
    {
        var dto = LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(requestV2.LinkedParticipants)
            .ToList();
        return dto;
    }
}
