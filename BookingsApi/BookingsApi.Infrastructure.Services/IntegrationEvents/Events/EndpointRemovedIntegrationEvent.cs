using System;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointRemovedIntegrationEvent : IIntegrationEvent
    {
        public EndpointRemovedIntegrationEvent(Guid hearingId, string sip)
        {
            HearingId = hearingId;
            Sip = sip;
        } 
        
        public Guid HearingId { get; }
        public string Sip { get; }
    }
}