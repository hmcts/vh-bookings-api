using Bookings.DAL.Dtos;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    /// <summary>
    /// This class is used to map a linked participant object to a linked participant dto
    /// used by the CloneHearingToCommandMapper.
    /// </summary>
    public static class LinkedParticipantToLinkedParticipantDtoMapper
    {
        public static LinkedParticipantDto MapToDto(this LinkedParticipant linkedParticipant)
        {
            return new LinkedParticipantDto
            {
                ParticipantContactEmail = linkedParticipant.
            };
        }
    }
}