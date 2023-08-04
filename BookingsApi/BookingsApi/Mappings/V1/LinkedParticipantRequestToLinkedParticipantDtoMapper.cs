using BookingsApi.Contract.V1.Requests;
using BookingsApi.Mappings.V1.Extensions;

namespace BookingsApi.Mappings.V1
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
                    var dto = new LinkedParticipantDto(request.ParticipantContactEmail,
                        request.LinkedParticipantContactEmail, request.Type.MapToDomainEnum());
                    listOfDtos.Add(dto);
                }
            }
            
            return listOfDtos;
        }
    }
}