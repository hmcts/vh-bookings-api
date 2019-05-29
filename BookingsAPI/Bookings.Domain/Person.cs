using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Ddd;
using Bookings.Domain.Validations;

namespace Bookings.Domain
{
    public class Person : AggregateRoot<Guid>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();

        public Person(string title, string firstName, string lastName, string username)
        {
            Id = Guid.NewGuid();
            ValidateArguments(firstName, lastName, username);
            Title = title;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            CreatedDate = DateTime.UtcNow;
        }

        public Person(string title, string firstName, string lastName, string username, Address address) : this(title, firstName, lastName, username)
        {
            Address = address;
        }

        public string Title { get; set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string MiddleNames { get; set; }
        public string Username { get; protected set; }
        public string ContactEmail { get; set; }
        public string TelephoneNumber { get; set; }
        protected long? OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        protected long? AddressId { get; set; }
        public virtual Address Address { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public virtual IList<SuitabilityAnswer> SuitabilityAnswers { get; set; }

        public void UpdateAddress(Address address)
        {
            Address = address;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdateOrganisation(Organisation organisation)
        {
            Organisation = organisation;
            UpdatedDate = DateTime.UtcNow;
        }

        private void ValidateArguments(string firstName, string lastName, string username)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                _validationFailures.AddFailure("FirstName", "FirstName cannot be empty");
            }
            if (string.IsNullOrEmpty(lastName))
            {
                _validationFailures.AddFailure("LastName", "LastName cannot be empty");
            }
            if (string.IsNullOrEmpty(username))
            {
                _validationFailures.AddFailure("Username", "Username cannot be empty");
            }

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }
    }
}
