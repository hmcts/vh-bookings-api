using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class LinkedParticipantDtoMapper
    {
        public static List<LinkedParticipantDto> MapToDto(IList<LinkedParticipant> linkedParticipants)
        {
            var listOfDtos = new List<LinkedParticipantDto>();
            
            if (linkedParticipants != null)
            {
                foreach (var linkedParticipant in linkedParticipants)
                {
                    var dto = new LinkedParticipantDto(linkedParticipant.ParticipantId, linkedParticipant.LinkedId, linkedParticipant.Type);
                
                    listOfDtos.Add(dto);
                }
            }

            return listOfDtos;
        }
    }
}