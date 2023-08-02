using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Requests
{
    public class LinkedParticipantRequest
    {
        public string ParticipantContactEmail { get; set; }
        public string LinkedParticipantContactEmail { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}