using Bookings.Domain.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Domain.Helpers
{
    public static class CommonValidations
    {
        public static ValidationFailures ValidateAddressDetails(string houseNumber, string street, string city, string county, string postcode)
        {
            ValidationFailures _validationFailures = new ValidationFailures();

            if (string.IsNullOrEmpty(houseNumber))
            {
                _validationFailures.AddFailure("HouseNumber", "HouseNumber is required");
            }
            if (string.IsNullOrEmpty(street))
            {
                _validationFailures.AddFailure("Street", "Street is required");
            }
            if (string.IsNullOrEmpty(city))
            {
                _validationFailures.AddFailure("City", "City is required");
            }
            if (string.IsNullOrEmpty(county))
            {
                _validationFailures.AddFailure("County", "County is required");
            }
            if (string.IsNullOrEmpty(postcode))
            {
                _validationFailures.AddFailure("Postcode", "Postcode is required");
            }

            return _validationFailures;

        }
    }
}
