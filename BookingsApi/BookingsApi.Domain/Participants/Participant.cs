﻿using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Extensions;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain.Participants
{
    public abstract class Participant : ParticipantBase
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
            CaseRoleId = caseRole?.Id;
        }
        
        public int? CaseRoleId { get; set; }
        public virtual CaseRole CaseRole { get; set; }
        public int HearingRoleId { get; set; }
        public HearingRole HearingRole { get; set; }
        public Guid PersonId { get; protected set; }
        public Person Person { get; protected set; }
        public Guid HearingId { get; set; }
        public virtual Hearing Hearing { get; protected set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int? InterpreterLanguageId { get; protected set; }
        public virtual InterpreterLanguage InterpreterLanguage { get; protected set; }
        public string OtherLanguage { get; set; }
        public IList<LinkedParticipant> LinkedParticipants { get; set; }
        
        public Guid? ScreeningId { get; protected set; }
        public virtual Screening Screening { get; protected set; }

        protected virtual void ValidateParticipantDetails(string title, string displayName, string telephoneNumber, string organisationName)
        {
            ValidateArguments(displayName);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }

        public virtual void UpdateParticipantDetails(string title, string displayName, string telephoneNumber, string organisationName,
            string contactEmail = null)
        {
            ValidateParticipantDetails(title, displayName, telephoneNumber, organisationName);

            DisplayName = displayName;
            Person.UpdatePerson(Person.FirstName, Person.LastName, title, telephoneNumber, contactEmail: contactEmail);
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

        public void UpdateParticipantDetails(string firstName, string lastName, string middleNames = null)
        {
            Person.UpdatePersonNames(firstName, lastName, middleNames: middleNames);
        }

        public void AddLink(Guid linkedId, LinkedParticipantType linkType)
        {
            var existingLink = LinkedParticipants.SingleOrDefault(x => x.LinkedId == linkedId && x.Type == linkType);
            if (existingLink == null)
            {
                LinkedParticipants.Add(new LinkedParticipant(Id, linkedId, linkType));
            }            
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

        public bool DoesPersonAlreadyExist()
        {
            return Person?.CreatedDate.TrimMilliseconds() != CreatedDate.TrimMilliseconds()
                   || Person.UpdatedDate.TrimMilliseconds() != CreatedDate.TrimMilliseconds()
                   || (Person.Username is not null && Person.Username != Person.ContactEmail);
        }

        public void ChangePerson(Person newPerson)
        {
            Person = newPerson;
        }
        
        public void UpdateLanguagePreferences(InterpreterLanguage language, string otherLanguage)
        {
            if (language != null && !string.IsNullOrEmpty(otherLanguage))
            {
                throw new DomainRuleException(nameof(Participant), DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
            }
            InterpreterLanguage = language;
            OtherLanguage = otherLanguage;
            
            UpdatedDate = DateTime.UtcNow;
        }
        
        public void AssignScreening(ScreeningType type, List<Participant> participants, List<Endpoint> endpoints)
        {
            var screening = Screening;
            if (screening == null)
            {
                screening = new Screening(type, this);
                Screening = screening;
                ScreeningId = screening.Id;
            }
            else
            {
                screening.ScreeningEntities.Clear();
            }

            foreach (var participant in participants)
            {
                screening.AddParticipant(participant);
            }

            foreach (var endpoint in endpoints)
            {
                screening.AddEndpoint(endpoint);
            }
        }

        public void RemoveScreening()
        {
            Screening.ScreeningEntities.Clear();
            Screening = null;
            ScreeningId = null;
        }
    }
}