using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingAllocationDtoMapper
    {
        public static HearingAllocationDto MapToDto(Hearing hearing)
        {
            var @case = hearing.GetCases()[0];
            var judge = hearing.GetJudge();

            var dto = new HearingAllocationDto();
            dto.HearingId = hearing.Id;
            dto.GroupId = hearing.SourceId;
            dto.ScheduledDateTime = hearing.ScheduledDateTime;
            dto.ScheduledDuration = hearing.ScheduledDuration;
            dto.CaseType = hearing.CaseType.Name;
            dto.CaseNumber = @case.Number;
            dto.CaseName = @case.Name;
            dto.HearingVenueName = hearing.HearingVenue.Name;
            dto.RecordAudio = hearing.AudioRecordingRequired;
            dto.JudgeDisplayName = judge?.DisplayName;
            
            return dto;
        }
    }
}