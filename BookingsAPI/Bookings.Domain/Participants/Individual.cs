using Bookings.Domain.Helpers;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using System.Linq;

namespace Bookings.Domain.Participants
{
    public class Individual : Participant
    {
        protected Individual()
        {
        }

        public Individual(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {

        }

        protected override void ValidatePartipantDetails(string title, string displayName, string telephoneNumber, string street, string houseNumber, string city, string county, string postcode, string organisationName)
        {
            ValidateArguments(displayName);

            var addressFailures = CommonValidations.ValidateAddressDetails(houseNumber, street, city, county, postcode);
            if (addressFailures.Any())
            {
                _validationFailures.AddRange(addressFailures);
            }

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }

        public override void UpdateParticipantDetails(string title, string displayName, string telephoneNumber, string street, string houseNumber, string city, string county, string postcode, string organisationName)
        {
            base.UpdateParticipantDetails(title, displayName, telephoneNumber, street, houseNumber, city, county, postcode, organisationName);
            if (Person.Address != null)
            {
                var address = Person.Address;
                address.HouseNumber = houseNumber;
                address.Street = street;
                address.City = city;
                address.County = county;
                address.Postcode = postcode;
                Person.UpdateAddress(address);
            }
            else
            {
                var newAddress = new Address(houseNumber, street, postcode, city, county);
                Person.UpdateAddress(newAddress);
            }
        }
    }
}