using System;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;

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
    }
}