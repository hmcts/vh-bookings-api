using Bookings.Domain.Ddd;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Enumerations;

namespace Bookings.Domain.Participants
{
    public abstract class Participant : Entity<Guid>
    {
        protected readonly ValidationFailures _validationFailures = new ValidationFailures();

        protected Participant()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            LinkedParticipants = new List<LinkedParticipant>();
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
        public Guid HearingId { get; set; }
        public virtual Hearing Hearing { get; protected set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public virtual Questionnaire Questionnaire { get; set; }
        public virtual IList<LinkedParticipant> LinkedParticipants { get; set; }


        protected virtual void ValidatePartipantDetails(string title, string displayName, string telephoneNumber, string organisationName)
        {
            ValidateArguments(displayName);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }

        public virtual void UpdateParticipantDetails(string title, string displayName, string telephoneNumber, string organisationName)
        {
            ValidatePartipantDetails(title, displayName, telephoneNumber, organisationName);

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

        public void AddLink(Guid linkedId, LinkedParticipantType linkType)
        {
            var existingLink = LinkedParticipants.SingleOrDefault(x => x.LinkedId == linkedId && x.Type == linkType);
            if (existingLink != null)
            {
                throw new DomainRuleException("LinkedParticipant", "Participant is already linked with the same link type");
            }
            LinkedParticipants.Add(new LinkedParticipant(Id, linkedId, linkType));
        }

        public void RemoveLink(LinkedParticipant linkedParticipant)
        {
            var link = LinkedParticipants.SingleOrDefault(
                x => x.LinkedId == linkedParticipant.LinkedId && x.Type == linkedParticipant.Type);
            if (link == null)
            {
                throw new DomainRuleException("LinkedParticipant", "Link does not exist");
            }

            LinkedParticipants.Remove(linkedParticipant);
        }

        public virtual IList<LinkedParticipant> GetLinkedParticipants()
        {
            return LinkedParticipants;
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