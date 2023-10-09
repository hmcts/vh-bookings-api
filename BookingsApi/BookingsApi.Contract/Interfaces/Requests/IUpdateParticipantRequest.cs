using System;

namespace BookingsApi.Contract.Interfaces.Requests
{
    public interface IUpdateParticipantRequest : IRepresentativeInfoRequest
    {
        public Guid ParticipantId { get; set; }
    }
}
