using Bookings.Domain.Ddd;
using Bookings.Domain.Validations;
using System.Linq;
using Bookings.Domain.Helpers;

namespace Bookings.Domain
{
    public class Address : Entity<long>
    {

        public Address()
        {

        }
        public Address(string houseNumber, string street, string postCode, string city, string county)
        {
            var failures = CommonValidations.ValidateAddressDetails(houseNumber, street, city, postCode, county);
            if (failures.Any())
            {
                throw new DomainRuleException(failures);
            }

            HouseNumber = houseNumber;
            Street = street;
            Postcode = postCode;
            City = city;
            County = county;

        }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public string County { get; set; }

    }
}
