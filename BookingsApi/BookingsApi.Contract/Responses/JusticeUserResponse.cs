using System;

namespace BookingsApi.Contract.Responses
{
    public class JusticeUserResponse
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        public int UserRoleId { get; set; }
        public string UserRoleName { get; set; }
        public bool IsJudicialOfficeHolder { get; set; }
        public string CreatedBy { get; set; }
    }
}