using System;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Domain
{
    [ExcludeFromCodeCoverage]
    public class JusticeUser : TrackableEntity<Guid>
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        public int UserRole { get; set; }
        public string CreatedBy { get; set; }
    }
}