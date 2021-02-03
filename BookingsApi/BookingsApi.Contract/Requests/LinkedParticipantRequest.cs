using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Contract.Requests
{
    public class LinkedParticipantRequest
    {
        public string ParticipantContactEmail { get; set; }
        public string LinkedParticipantContactEmail { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}