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

            return new HearingAllocationDto
            {
                HearingId = hearing.Id,
                GroupId = hearing.SourceId,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                CaseType = hearing.CaseType.Name,
                CaseNumber = @case.Number,
                CaseName= @case.Name,
                HearingVenueName = hearing.HearingVenue.Name,
                RecordAudio = hearing.AudioRecordingRequired,
                HearingType = hearing.HearingType.Name,
                JudgeDisplayName = judge?.DisplayName
            };
        }
    }
}