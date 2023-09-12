using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2
{
    /// <summary>
    /// This class is used to map a linked participant request object to a linked participant dto
    /// </summary>
    public static class LinkedParticipantRequestV2ToLinkedParticipantDtoMapper
    {
        public static List<LinkedParticipantDto> MapToDto(List<LinkedParticipantRequestV2> requests)
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