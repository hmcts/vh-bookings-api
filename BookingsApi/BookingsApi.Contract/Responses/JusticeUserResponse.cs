using System;
using System.Collections.Generic;
using BookingsApi.Contract.Requests.Enums;

namespace BookingsApi.Contract.Responses
{
    public class JusticeUserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        
        public List<JusticeUserRole> UserRoles { get; set; }
        public bool IsVhTeamLeader { get; set; }
        public string CreatedBy { get; set; }
        public string FullName { get; set; }
        public bool Deleted { get; set; }
    }
}