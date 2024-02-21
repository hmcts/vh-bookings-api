using System;

namespace BookingsApi.Contract.Interfaces.Requests
{
    public interface IUpdateParticipantRequest : IRepresentativeInfoRequest
    {
        public Guid ParticipantId { get; set; }
        public string DisplayName { get; set; }
        public string TelephoneNumber { get; set; }
        public string Title { get; set; }
    }
}
