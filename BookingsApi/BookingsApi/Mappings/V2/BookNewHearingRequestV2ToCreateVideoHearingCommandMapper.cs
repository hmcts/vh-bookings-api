using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2;

internal static class BookNewHearingRequestV2ToCreateVideoHearingCommandMapper
{
    internal static CreateVideoHearingCommand Map(
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

        var conferenceSupplier = requestV2.BookingSupplier?.MapToDomainEnum() ?? VideoSupplier.Vodafone;
        return new CreateVideoHearingCommand(
            new CreateVideoHearingRequiredDto(caseType, requestV2.ScheduledDateTime,
                requestV2.ScheduledDuration, venue, cases, conferenceSupplier),
            new CreateVideoHearingOptionalDto(newParticipants, requestV2.HearingRoomName, requestV2.OtherInformation,
                requestV2.CreatedBy, requestV2.AudioRecordingRequired, newEndpoints, linkedParticipants,
                judiciaryParticipants, requestV2.IsMultiDayHearing, null)
        );
    }

    private static List<NewJudiciaryParticipant> MapJudiciaryParticipants(BookNewHearingRequestV2 requestV2)
    {
        return requestV2.JudicialOfficeHolders.Select(JudiciaryParticipantRequestToNewJudiciaryParticipantMapper.Map).ToList();
    }

    private static List<NewParticipant> MapParticipants(BookNewHearingRequestV2 requestV2, List<HearingRole> hearingRoles)
    {
        var newParticipants = requestV2.Participants.Select(
            x => ParticipantRequestV2ToNewParticipantMapper.Map(x, hearingRoles)).ToList();
        return newParticipants;
    }

    private static List<NewEndpoint> MapEndpoints(BookNewHearingRequestV2 requestV2, IRandomGenerator randomGenerator,
        string sipAddressStem)
    {
        var dtos = requestV2.Endpoints.Select(x => new NewEndpointRequestDto()
        {
            ExternalReferenceId = x.ExternalParticipantId,
            MeasuresExternalId = x.MeasuresExternalId,
            DisplayName = x.DisplayName,
            LinkedParticipantEmails = GetLinkedParticipants(x),
            OtherLanguage = x.OtherLanguage,
            InterpreterLanguageCode = x.InterpreterLanguageCode,
            Screening = x.Screening?.MapToDalDto()
        }).ToList();
        
        return NewEndpointGenerator.GenerateNewEndpoints(dtos, randomGenerator, sipAddressStem);
    }

    private static List<string> GetLinkedParticipants(EndpointRequestV2 x)
        => String.IsNullOrWhiteSpace(x.DefenceAdvocateContactEmail) 
            ? x.LinkedParticipantEmails
            : new List<string> {x.DefenceAdvocateContactEmail}.Concat(x.LinkedParticipantEmails).ToList();
    

    private static List<LinkedParticipantDto> MapLinkedParticipants(BookNewHearingRequestV2 requestV2)
        => LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(requestV2.LinkedParticipants).ToList();
}
