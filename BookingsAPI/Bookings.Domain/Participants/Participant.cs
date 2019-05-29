using System;
using System.Linq;
using Bookings.Domain.Ddd;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using Bookings.Domain.Helpers;
using System.Collections.Generic;

namespace Bookings.Domain.Participants
{
    public abstract class Participant : Entity<Guid>
    {
        protected readonly ValidationFailures _validationFailures = new ValidationFailures();

        protected Participant()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        protected Participant(Person person, HearingRole hearingRole, CaseRole caseRole) : this()
        {
            Person = person;
            PersonId = person.Id;
            HearingRoleId = hearingRole.Id;
            CaseRoleId = caseRole.Id;
        }

        public string DisplayName { get; set; }
        public int CaseRoleId { get; set; }
        public virtual CaseRole CaseRole { get; set; }
        public int HearingRoleId { get; set; }
        public virtual HearingRole HearingRole { get; set; }
        public Guid PersonId { get; protected set; }
        public virtual Person Person { get; protected set; }
        public Guid HearingId { get; protected set; }
        protected virtual Hearing Hearing { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public virtual IList<SuitabilityAnswer> SuitabilityAnswers { get; set; }


        protected virtual void ValidatePartipantDetails(string title, string displayName, string telephoneNumber, string street, string houseNumber, string city, string county, string postcode, string organisationName)
        {
            ValidateArguments(displayName);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }

        public virtual void UpdateParticipantDetails(string title, string displayName, string telephoneNumber, string street, string houseNumber, string city, string county, string postcode, string organisationName)
        {
            ValidatePartipantDetails(title, displayName, telephoneNumber, street, houseNumber, city, county, postcode, organisationName);

            DisplayName = displayName;
            Person.Title = title;
            Person.TelephoneNumber = telephoneNumber;
            UpdatedDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(organisationName))
            {
                var organisation = Person.Organisation;
                if (organisation != null)
                {
                    organisation.Name = organisationName;
                }
                else
                {
                    organisation = new Organisation(organisationName);
                }
                Person.UpdateOrganisation(organisation);
            }
        }

        protected void ValidateArguments(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                _validationFailures.AddFailure("DisplayName", "DisplayName is required");
            }
        }

    }
}