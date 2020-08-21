using System;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointRemovedIntegrationEvent : IIntegrationEvent
    {
        public EndpointRemovedIntegrationEvent(Guid hearingId, Guid endpointId)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
        } 
        
        public Guid HearingId { get; }
        public Guid EndpointId { get; }
    }
}