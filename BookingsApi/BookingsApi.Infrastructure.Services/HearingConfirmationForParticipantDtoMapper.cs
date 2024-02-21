using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public abstract class HearingConfirmationForParticipantDtoMapper
    {
        public static IEnumerable<HearingConfirmationForParticipantDto> MapToDtos(Hearing hearing)
        {
            var participantDtos = hearing.Participants
                .Select(p => ParticipantDtoMapper.MapToDto(p, hearing.OtherInformation))
                .ToList();
            var judiciaryParticipantDtos = hearing.JudiciaryParticipants
                .Select(ParticipantDtoMapper.MapToDto)
                .ToList();
            var allParticipantDtos = participantDtos.Union(judiciaryParticipantDtos).ToList();
            var @case = hearing.GetCases()[0];
            var hearingConfirmationDtos = allParticipantDtos
                .Select(p => EventDtoMappers.MapToHearingConfirmationDto(
                    hearing.Id, hearing.ScheduledDateTime, p, @case))
                .ToList();

            return hearingConfirmationDtos;
        }
    }
}
