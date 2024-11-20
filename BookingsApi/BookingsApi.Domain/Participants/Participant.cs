using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Extensions;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain.Participants;

public abstract class Participant : ParticipantBase, IScreenableEntity
{
    protected readonly ValidationFailures ValidationFailures = new();

    protected Participant()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        LinkedParticipants = new List<LinkedParticipant>();
    }

    protected Participant(string externalReferenceId, Person person, HearingRole hearingRole, string displayName) : this()
    {
        Person = person;
        PersonId = person.Id;
        HearingRoleId = hearingRole.Id;
        ExternalReferenceId = externalReferenceId;
        DisplayName = displayName;
    }
    
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
    public string ExternalReferenceId { get; set; }
    public string MeasuresExternalId { get; set; }
    public int? InterpreterLanguageId { get; protected set; }
    public virtual InterpreterLanguage InterpreterLanguage { get; protected set; }
    public string OtherLanguage { get; set; }
    public IList<LinkedParticipant> LinkedParticipants { get; set; }
        
    public Guid? ScreeningId { get; set; }
    public virtual Screening Screening { get; set; }

    protected virtual void ValidateParticipantDetails(string title, string displayName, string telephoneNumber, string organisationName)
    {
        ValidateArguments(displayName);

        if (ValidationFailures.Any())
        {
            throw new DomainRuleException(ValidationFailures);
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
            ValidationFailures.AddFailure("DisplayName", "DisplayName is required");
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
        ScreeningHelper.AssignScreening(this, type, participants, endpoints);
        UpdatedDate = DateTime.UtcNow;
    }

    public void RemoveScreening()
    {
        ScreeningHelper.RemoveScreening(this);
        UpdatedDate = DateTime.UtcNow;
    }
}