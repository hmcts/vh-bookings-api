using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingDtoMapper
    {
        public static HearingAllocationDto MapToDto(Hearing hearing)
        {
            var @case = hearing.GetCases().First();
            return new HearingAllocationDto
            {
                HearingId = hearing.Id,
                GroupId = hearing.SourceId,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                CaseType = hearing.CaseType.Name,
                CaseNumber = @case?.Number,
                CaseName= @case?.Name,
                HearingVenueName = hearing.HearingVenueName,
                RecordAudio = hearing.AudioRecordingRequired,
                HearingType = hearing.HearingType.Name,
                JudgeDisplayName = hearing.Participants?.FirstOrDefault(p => p.HearingRole.UserRole.IsJudge)?.DisplayName
            };
        }
    }
}