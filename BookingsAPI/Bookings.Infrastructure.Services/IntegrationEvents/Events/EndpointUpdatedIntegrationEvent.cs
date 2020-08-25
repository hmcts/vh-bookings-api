using System;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public EndpointUpdatedIntegrationEvent(Guid hearingId, Guid endpointId, string displayName)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
        }
        
        public Guid HearingId { get; }
        public Guid EndpointId { get; }
        public string DisplayName { get; }
    }
}