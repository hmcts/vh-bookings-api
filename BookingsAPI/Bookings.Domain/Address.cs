using Bookings.Domain.Ddd;

namespace Bookings.Domain
{
    public class Address : Entity<long>
    {
        public Address()
        {

        }
        public Address(string houseNumber, string street, string postCode)
        {
            HouseNumber = houseNumber;
            Street = street;
            Postcode = postCode;
        }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string Postcode { get; set; }
    }
}