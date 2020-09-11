using System;
using Bookings.Domain.Ddd;
using Bookings.Domain.Participants;

namespace Bookings.Domain
{
    public class Endpoint : AggregateRoot<Guid>
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public Guid HearingId { get; set; }
        public Hearing Hearing { get; set; }
        public Participant DefenceAdvocate { get; private set; }

        public Endpoint(string displayName, string sip, string pin)
        {
            DisplayName = displayName;
            Sip = sip;
            Pin = pin;
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