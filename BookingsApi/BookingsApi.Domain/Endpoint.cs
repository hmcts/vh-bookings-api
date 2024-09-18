using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public class Endpoint : TrackableEntity<Guid>
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
        
        public Guid? ScreeningId { get; protected set; }
        public virtual Screening Screening { get; protected set; }
        
        protected Endpoint(){}

        public Endpoint(string displayName, string sip, string pin, Participant defenceAdvocate)
        {
            DisplayName = displayName;
            Sip = sip;
            Pin = pin;
            DefenceAdvocate = defenceAdvocate;
        }

        public void AssignDefenceAdvocate(Participant defenceAdvocate)
        {
            DefenceAdvocate = defenceAdvocate;
        }
        
        public void UpdateDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            DisplayName = displayName;
        }
        
        public void UpdateLanguagePreferences(InterpreterLanguage language, string otherLanguage)
        {
            if (language != null && !string.IsNullOrEmpty(otherLanguage))
            {
                throw new DomainRuleException(nameof(Endpoint), DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
            }
            InterpreterLanguage = language;
            OtherLanguage = otherLanguage;

            UpdatedDate = DateTime.UtcNow;
        }

        public void AssignScreening(ScreeningType type, List<Participant> participants, List<Endpoint> endpoints)
        {
            var screening = new Screening(type, this);
            foreach (var participant in participants)
            {
                screening.AddParticipant(participant);
            }

            foreach (var endpoint in endpoints)
            {
                screening.AddEndpoint(endpoint);
            }
            Screening = screening;
        }

        public void RemoveScreening()
        {
            Screening.ScreeningEntities.Clear();
            Screening = null;
            ScreeningId = null;
        }
    }
}