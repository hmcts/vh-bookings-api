using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Requests
{
    public class LinkedParticipantRequestV2
    {
        public string ParticipantContactEmail { get; set; }
        public string LinkedParticipantContactEmail { get; set; }
        public LinkedParticipantTypeV2 TypeV2 { get; set; }
    }
}