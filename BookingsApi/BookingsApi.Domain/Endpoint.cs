using System;
using System.Collections.Generic;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain;

public class Endpoint : TrackableEntity<Guid>, IScreenableEntity
{
    public string DisplayName { get; set; }
    public string Sip { get; set; }
    public string Pin { get; set; }
    public Participant DefenceAdvocate { get; private set; }
    public Guid HearingId { get; set; }
    public virtual Hearing Hearing { get; protected set; }
    public int? InterpreterLanguageId { get; protected set; }
    public virtual InterpreterLanguage InterpreterLanguage { get; protected set; }
    public string OtherLanguage { get; set; }
    public string ExternalReferenceId { get; protected set; }
    public string MeasuresExternalId { get; protected set; }
    public Guid? ScreeningId { get; set; }
    public virtual Screening Screening { get; set; }

    protected Endpoint()
    {
    }

    public Endpoint(string externalReferenceId, string displayName, string sip, string pin, Participant defenceAdvocate)
    {
        DisplayName = displayName;
        Sip = sip;
        Pin = pin;
        DefenceAdvocate = defenceAdvocate;
        ExternalReferenceId = externalReferenceId;
    }

    public void AssignDefenceAdvocate(Participant defenceAdvocate)
    {
        if (defenceAdvocate?.Id == DefenceAdvocate?.Id) return;
        
        DefenceAdvocate = defenceAdvocate;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateDisplayName(string displayName)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            throw new ArgumentNullException(nameof(displayName));
        }

        if (displayName == DisplayName) return;
        
        DisplayName = displayName;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateLanguagePreferences(InterpreterLanguage language, string otherLanguage)
    {
        if (language != null && !string.IsNullOrEmpty(otherLanguage))
        {
            throw new DomainRuleException(nameof(Endpoint),
                DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
        }

        if (language?.Id == InterpreterLanguage?.Id && otherLanguage == OtherLanguage) return;

        InterpreterLanguage = language;
        OtherLanguage = otherLanguage;

        UpdatedDate = DateTime.UtcNow;
    }

    public void AssignScreening(ScreeningType type, List<Participant> participants, List<Endpoint> endpoints)
    {
        var originalScreeningUpdatedDate = Screening?.UpdatedDate;
        
        ScreeningHelper.AssignScreening(this, type, participants, endpoints);

        if (Screening?.UpdatedDate == originalScreeningUpdatedDate) return;
        
        UpdatedDate = DateTime.UtcNow;
    }

    public void RemoveScreening()
    {
        ScreeningHelper.RemoveScreening(this);
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateExternalIds(string externalReferenceId, string measuresExternalId)
    {
        if (externalReferenceId == ExternalReferenceId && measuresExternalId == MeasuresExternalId) return;
        
        ExternalReferenceId = externalReferenceId;
        MeasuresExternalId = measuresExternalId;
        UpdatedDate = DateTime.UtcNow;
    }
}