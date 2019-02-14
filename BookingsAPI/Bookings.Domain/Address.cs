using Bookings.Domain.Ddd;

namespace Bookings.Domain
{
    public class Address : Entity<long>
    {
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string Postcode { get; set; }
    }
}