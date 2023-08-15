namespace BookingsApi.Contract.V1.Requests
{
    public class JudiciaryLeaverRequest
    {
        public string Id { get; set; }
        public bool Leaver { get; set; }
        public string LeftOn { get; set; }
        public string PersonalCode { get; set; }
    }
}