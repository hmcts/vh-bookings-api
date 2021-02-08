using BookingsApi.Domain.Enumerations;

namespace BookingsApi.DAL.Dtos
{
    public class LinkedParticipantDto
    {
        public string ParticipantContactEmail { get; set; }
        public string LinkedParticipantContactEmail { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}