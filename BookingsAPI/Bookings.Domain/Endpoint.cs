using System;
using Bookings.Domain.Ddd;
using Bookings.Domain.Participants;

namespace Bookings.Domain
{
    public class Endpoint : Entity<Guid>
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public Participant DefenceAdvocate { get; private set; }
        // private Endpoint()
        // {
        //     Id = Guid.NewGuid();
        // }
        
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