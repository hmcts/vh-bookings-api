using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class LinkedParticipantDtoMapper
    {
        public static LinkedParticipantDto MapToDto(LinkedParticipant linkedParticipant)
        {
            return new LinkedParticipantDto
            {
                ParticipantId = linkedParticipant.ParticipantId,
                LinkedId = linkedParticipant.LinkedId,
                Type = linkedParticipant.Type
            };
        }

    }
}