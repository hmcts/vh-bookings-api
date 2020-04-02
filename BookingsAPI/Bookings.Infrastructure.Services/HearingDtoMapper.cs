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
            return new HearingDto(hearing.Id, hearing.ScheduledDateTime, hearing.ScheduledDuration,
                hearing.CaseType.Name, @case.Number, @case.Name, hearing.HearingVenueName)
            {
                RecordAudio = hearing.AudioRecordingRequired
            };
        }
    }
}