using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingAllocationDtoMapper
    {
        public static HearingAllocationDto MapToDto(Hearing hearing)
        {
            var @case = hearing.GetCases()[0];
            var judge = hearing.Participants?.FirstOrDefault(p => p.HearingRole.UserRole.IsJudge);
            var judiciaryJudge = hearing.JudiciaryParticipants?.FirstOrDefault(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);

            return new HearingAllocationDto
            {
                HearingId = hearing.Id,
                GroupId = hearing.SourceId,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                CaseType = hearing.CaseType.Name,
                CaseNumber = @case.Number,
                CaseName= @case.Name,
                HearingVenueName = hearing.HearingVenueName,
                RecordAudio = hearing.AudioRecordingRequired,
                HearingType = hearing.HearingType.Name,
                JudgeDisplayName = judge != null ? judge.DisplayName : judiciaryJudge?.DisplayName
            };
        }
    }
}