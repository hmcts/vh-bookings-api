namespace BookingsApi.Contract.Interfaces.Requests
{
    public interface IParticipantRequest : IRepresentativeInfoRequest
    {
        public string ContactEmail { get; set; }
    }
}
