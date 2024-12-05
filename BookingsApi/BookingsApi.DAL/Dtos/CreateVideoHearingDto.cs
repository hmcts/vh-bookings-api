using BookingsApi.DAL.Commands;

namespace BookingsApi.DAL.Dtos;

public record CreateVideoHearingRequiredDto(CaseType CaseType, DateTime ScheduledDateTime,
    int ScheduledDuration, HearingVenue Venue, List<Case> Cases, VideoSupplier Supplier);
    
public record CreateVideoHearingOptionalDto(List<NewParticipant> Participants, string HearingRoomName,
    string OtherInformation, string CreatedBy, bool AudioRecordingRequired, List<NewEndpoint> Endpoints, List<LinkedParticipantDto> LinkedParticipants,
    List<NewJudiciaryParticipant> JudiciaryParticipants, bool IsMultiDayFirstHearing, Guid? SourceId);