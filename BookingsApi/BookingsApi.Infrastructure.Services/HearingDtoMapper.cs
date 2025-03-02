using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;
using BookingsApi.Domain.Constants;

namespace BookingsApi.Infrastructure.Services
{
    public static class HearingDtoMapper
    {
        public static HearingDto MapToDto(Hearing hearing)
        {
            var roomType =
                hearing.GetParticipants().Any(x => x.Screening?.ScreeningEntities.Count > 0) ||
                hearing.GetEndpoints().Any(x => x.Screening?.ScreeningEntities.Count > 0)
                    ? ConferenceRoomType.VA
                    : ConferenceRoomType.VMR;
            
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
                IsVenueWelsh = hearing.HearingVenue.IsWelsh(),
                RecordAudio = hearing.AudioRecordingRequired,
                CaseTypeServiceId = hearing.CaseType.ServiceId ?? RefData.DefaultCaseTypeServiceId,
                VideoSupplier = hearing.ConferenceSupplier,
                ConferenceRoomType = roomType,
            };
        }
        
        
    }
}