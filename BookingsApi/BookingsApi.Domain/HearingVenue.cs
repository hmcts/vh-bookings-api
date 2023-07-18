using System.Linq;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public class HearingVenue : TrackableEntity<int>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();

        public HearingVenue(int id, string name, bool isScottish = false, bool isWorkAllocationEnabled = true, string venueCode = null)
        {
            ValidateArguments(id, name);

            Id = id;
            Name = name;
            IsScottish = isScottish;
            IsWorkAllocationEnabled = isWorkAllocationEnabled;
            VenueCode = venueCode;
        }

        public string Name { get; }
        public string VenueCode { get; }
        public bool IsScottish { get; }
        public bool IsWorkAllocationEnabled { get; }

        private void ValidateArguments(int id, string name)
        {
            if (id <= 0)
            {
                _validationFailures.AddFailure(nameof(Id), "Id is not a valid value");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                _validationFailures.AddFailure(nameof(Name), "Name is not a valid value");
            }
            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }
    }
}