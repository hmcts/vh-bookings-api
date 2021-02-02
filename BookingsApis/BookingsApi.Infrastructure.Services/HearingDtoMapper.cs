using System.Linq;
using Bookings.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services
{
    public static class HearingDtoMapper
    {
        public static HearingDto MapToDto(Hearing hearing)
        {
            var @case = hearing.GetCases().First(); // Does this need to be a lead case?
            return new HearingDto
            {
                HearingId = hearing.Id,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                CaseType = hearing.CaseType.Name,
                CaseNumber = @case.Number,
                CaseName= @case.Name,
                HearingVenueName = hearing.HearingVenueName,
                RecordAudio = hearing.AudioRecordingRequired
            };
        }
    }
}