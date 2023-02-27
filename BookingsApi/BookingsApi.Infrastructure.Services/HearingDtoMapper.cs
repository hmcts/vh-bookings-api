using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingDtoMapper
    {
        public static HearingDto MapToDto(Hearing hearing)
        {
            var @case = hearing.GetCases().First();
            return new HearingDto
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
                HearingType = hearing.HearingType.Name
            };
        }
        
        
    }
}