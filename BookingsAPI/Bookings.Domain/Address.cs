using Bookings.Domain.Ddd;
using Bookings.Domain.Validations;
using System.Linq;

namespace Bookings.Domain
{
    public class Address : Entity<long>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();

        public Address()
        {

        }
        public Address(string houseNumber, string street, string postCode, string city, string county)
        {
            ValidateArguments(houseNumber, street, city, postCode, county);

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

        private void ValidateArguments(string houseNumber, string street, string city, string postcode, string county)
        {
               if (string.IsNullOrEmpty(houseNumber))
                {
                    _validationFailures.AddFailure("Housenumber", "Housenumber cannot be empty");
                }
                if (string.IsNullOrEmpty(street))
                {
                    _validationFailures.AddFailure("Street", "Street cannot be empty");
                }
                if (string.IsNullOrEmpty(city))
                {
                    _validationFailures.AddFailure("City", "City cannot be empty");
                }

                if (string.IsNullOrEmpty(county))
                {
                    _validationFailures.AddFailure("County", "County cannot be empty");
                }

                if (string.IsNullOrEmpty(postcode))
                {
                    _validationFailures.AddFailure("Postcode", "Username cannot be empty");
                }

                if (_validationFailures.Any())
                {
                    throw new DomainRuleException(_validationFailures);
                }
            
        }
    }
}