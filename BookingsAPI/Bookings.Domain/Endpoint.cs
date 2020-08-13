using System;
using Bookings.Domain.Ddd;

namespace Bookings.Domain
{
    public class Endpoint : AggregateRoot<Guid>
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public Guid HearingId { get; set; }
        public Hearing Hearing { get; set; }

        public Endpoint(string displayName, string sip, string pin)
        {
            DisplayName = displayName;
            Sip = sip;
            Pin = pin;
        }
    }
}