namespace BookingsApi.Contract.Interfaces.Requests
{
    public interface IParticipantRequest : IRepresentativeInfoRequest
    {
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
    }
}
