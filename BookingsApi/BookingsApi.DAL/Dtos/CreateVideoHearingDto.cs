using BookingsApi.DAL.Commands;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.DAL.Dtos;

public record CreateVideoHearingRequiredDto(CaseType CaseType, DateTime ScheduledDateTime,
    int ScheduledDuration, HearingVenue Venue, List<Case> Cases);
    
public record CreateVideoHearingOptionalDto(List<NewParticipant> Participants, string HearingRoomName,
    string OtherInformation, string CreatedBy, bool AudioRecordingRequired, List<NewEndpointDto> Endpoints,
    string CancelReason, List<LinkedParticipantDto> LinkedParticipants,
    List<NewJudiciaryParticipant> JudiciaryParticipants, bool IsMultiDayFirstHearing, Guid? SourceId, HearingType HearingType);