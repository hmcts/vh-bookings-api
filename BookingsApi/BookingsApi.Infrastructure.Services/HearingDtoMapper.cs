using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingDtoMapper
    {
        public static HearingDto MapToDto(Hearing hearing)
        {
            var @case = hearing.GetCases().First(); // Does this need to be a lead case?
            return new HearingDto
            {
                HearingId = hearing.Id,
                GroupId = hearing.SourceId.GetValueOrDefault(),
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                CaseType = hearing.CaseType.Name,
                CaseNumber = @case.Number,
                CaseName= @case.Name,
                HearingVenueName = hearing.HearingVenueName,
                RecordAudio = hearing.AudioRecordingRequired,
            };
        }
    }
}