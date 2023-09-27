namespace BookingsApi.Contract.Interfaces.Requests
{
    public interface IParticipantRequest : IRepresentativeInfoRequest
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string TelephoneNumber { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string CaseRoleName { get; set; }
        public string HearingRoleName { get; set; }
        public string Representee { get; set; }
        public string OrganisationName { get; set; }
    }
}
