using BookingsApi.Domain.RefData;
using System;

namespace BookingsApi.Domain
{
    public class JusticeUser : TrackableEntity<Guid>
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public string CreatedBy { get; set; }
    }
}