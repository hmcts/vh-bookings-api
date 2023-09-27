namespace BookingsApi.Contract.Interfaces.Requests
{
    public interface IUpdateParticipantRequest : IRepresentativeInfoRequest
    {
        public string ContactEmail { get; set; }
    }
}
