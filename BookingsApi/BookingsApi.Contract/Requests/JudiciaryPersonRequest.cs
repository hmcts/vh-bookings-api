using System;

namespace BookingsApi.Contract.Requests
{
    public class JudiciaryPersonRequest
    {
        public string Id { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public bool HasLeft { get; set; }
        public bool Leaver { get; set; }
        public string LeftOn { get; set; }
    }
}