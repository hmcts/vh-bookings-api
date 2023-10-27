using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;
using BookingsApi.Domain.Constants;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingDtoMapper
    {
        public static HearingDto MapToDto(Hearing hearing)
        {
            var @case = hearing.GetCases()[0];
            return new HearingDto
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
                HearingType = hearing.HearingType?.Name,
                CaseTypeServiceId = hearing.CaseType.ServiceId ?? RefData.DefaultCaseTypeServiceId
            };
        }
        
        
    }
}