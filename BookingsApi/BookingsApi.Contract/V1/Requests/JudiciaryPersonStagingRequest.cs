namespace BookingsApi.Contract.V1.Requests
{
    public class JudiciaryPersonStagingRequest
    {
        public string Id { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public string Leaver { get; set; }
        public string LeftOn { get; set; }
    }
}