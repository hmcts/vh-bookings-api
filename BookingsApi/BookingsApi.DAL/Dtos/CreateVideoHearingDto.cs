using BookingsApi.DAL.Commands;

namespace BookingsApi.DAL.Dtos;

public record CreateVideoHearingRequiredDto(CaseType CaseType, DateTime ScheduledDateTime,
    int ScheduledDuration, HearingVenue Venue, List<Case> Cases);
    
public record CreateVideoHearingOptionalDto(List<NewParticipant> Participants, string HearingRoomName,
    string OtherInformation, string CreatedBy, bool AudioRecordingRequired, List<NewEndpoint> Endpoints,
    string CancelReason, List<LinkedParticipantDto> LinkedParticipants,
    List<NewJudiciaryParticipant> JudiciaryParticipants, bool IsMultiDayFirstHearing, Guid? SourceId, HearingType HearingType);