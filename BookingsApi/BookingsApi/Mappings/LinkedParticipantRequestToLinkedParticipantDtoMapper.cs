using System.Collections.Generic;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Dtos;
using BookingsApi.Extensions;

namespace BookingsApi.Mappings
{
    /// <summary>
    /// This class is used to map a linked participant request object to a linked participant dto
    /// </summary>
    public static class LinkedParticipantRequestToLinkedParticipantDtoMapper
    {
        public static List<LinkedParticipantDto> MapToDto(List<LinkedParticipantRequest> requests)
        {
            var listOfDtos = new List<LinkedParticipantDto>();
            
            if (requests != null)
            {
                foreach (var request in requests)
                {
                    var dto = new LinkedParticipantDto
                    {
                        ParticipantContactEmail = request.ParticipantContactEmail,
                        LinkedParticipantContactEmail = request.LinkedParticipantContactEmail,
                        Type = request.Type.MapToDomainEnum()
                    };
                
                    listOfDtos.Add(dto);
                }
            }
            
            return listOfDtos;
        }
    }
}