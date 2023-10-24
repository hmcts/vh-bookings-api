using System;
using System.Linq;
using BookingsApi.Domain.Ddd;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public class Person : AggregateRoot<Guid>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();

        /// <summary>
        /// Instantiate a person when the username is known, typically used for existing persons
        /// </summary>
        public Person(string title, string firstName, string lastName, string contactEmail, string username)
        {
            Id = Guid.NewGuid();
            ValidateArguments(firstName, lastName, contactEmail);
            Title = title;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            ContactEmail = contactEmail;
            CreatedDate = UpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Instantiate a person when the username is not known, typically used for new persons
        /// </summary>
        public Person(string title, string firstName, string lastName, string contactEmail) : this(title, firstName,
            lastName, contactEmail, null)
        {
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

        public void UpdatePerson(string firstName, string lastName, string username)
        {
            UpdatePerson(firstName, lastName, null, null);
            Username = username;
        }

        public void UpdatePerson(string firstName, string lastName, string title = null, string telephoneNumber = null)
        {
            ValidateArguments(firstName, lastName, ContactEmail);
            FirstName = firstName;
            LastName = lastName;
            Title = title == null ? Title : title;
            TelephoneNumber = telephoneNumber == null ? TelephoneNumber : telephoneNumber;
            UpdatedDate = DateTime.UtcNow;

        }

        public void UpdatePerson(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                _validationFailures.AddFailure("Username", "Username cannot be empty");
            }
            Username = username;
        }

        public void UpdatePersonNames(string firstName, string lastName, string middleNames = null)
        {
            ValidateArguments(firstName, lastName);
            FirstName = firstName;
            LastName = lastName;
            if (!string.IsNullOrEmpty(middleNames))
                MiddleNames = middleNames;
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

        public void AnonymisePersonForSchedulerJob()
        {
            var randomString = RandomStringGenerator.GenerateRandomString(9);
            
            FirstName = randomString;
            LastName = randomString;
            MiddleNames = randomString;
            Username = $"{randomString}@email.net";
            ContactEmail = $"{randomString}@hmcts.net";
            TelephoneNumber = "00000000000";

            if (Organisation != null)
            {
                Organisation.Name = randomString;
            }
        }

        private void ValidateArguments(string firstName, string lastName, string contactEmail)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                _validationFailures.AddFailure("FirstName", DomainRuleErrorMessages.FirstNameRequired);
            }
            if (string.IsNullOrEmpty(lastName))
            {
                _validationFailures.AddFailure("LastName", DomainRuleErrorMessages.LastNameRequired);
            }
            if (string.IsNullOrEmpty(contactEmail))
            {
                _validationFailures.AddFailure("ContactEmail", "ContactEmail cannot be empty");
            }

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }

        private void ValidateArguments(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                _validationFailures.AddFailure("FirstName", DomainRuleErrorMessages.FirstNameRequired);
            }
            if (string.IsNullOrEmpty(lastName))
            {
                _validationFailures.AddFailure("LastName", DomainRuleErrorMessages.LastNameRequired);
            }
            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }
    }
}
