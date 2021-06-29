using System;
using System.Linq;
using BookingsApi.Domain.Ddd;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
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
            CreatedDate = UpdatedDate = DateTime.UtcNow;
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
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }

        public void UpdateOrganisation(Organisation organisation)
        {
            Organisation = organisation;
            UpdatedDate = DateTime.UtcNow;
        }

        public void UpdatePerson(string firstName, string lastName, string username, string title = null, string telephoneNumber = null)
        {
            ValidateArguments(firstName, lastName, username);
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Title = title == null ? Title : title;
            TelephoneNumber = telephoneNumber == null ? TelephoneNumber : telephoneNumber;
            UpdatedDate = DateTime.UtcNow;
        }

        public void AnonymisePerson()
        {
            var firstname = RandomStringGenerator.GenerateRandomString(10);
            var lastName = RandomStringGenerator.GenerateRandomString(10);
            var newUsername = $"{firstname}.{lastName}@hmcts.net";
            var contactEmail = $"{RandomStringGenerator.GenerateRandomString(10)}@hmcts.net";

            FirstName = firstname;
            LastName = lastName;
            Username = newUsername;
            ContactEmail = contactEmail;
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
