using System;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public EndpointUpdatedIntegrationEvent(Guid hearingId, string sip, string displayName)
        {
            HearingId = hearingId;
            Sip = sip;
            DisplayName = displayName;
        }
        
        public Guid HearingId { get; }
        public string Sip { get; }
        public string DisplayName { get; }
    }
}