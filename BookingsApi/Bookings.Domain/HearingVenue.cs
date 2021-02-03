﻿using System.Linq;
using Bookings.Domain.Ddd;
using Bookings.Domain.Validations;

namespace Bookings.Domain
{
    public class HearingVenue : Entity<int>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();

        public HearingVenue(int id, string name)
        {
            ValidateArguments(id, name);

            Id = id;
            Name = name;
        }

        public string Name { get; }

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