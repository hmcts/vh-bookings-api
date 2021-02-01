using Bookings.Domain.Enumerations;

namespace Bookings.Api.Contract.Requests
{
    public class LinkedParticipantRequest
    {
        public string ParticipantContactEmail { get; set; }
        public string LinkedParticipantContactEmail { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}