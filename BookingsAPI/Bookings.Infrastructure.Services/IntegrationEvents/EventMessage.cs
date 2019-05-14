using System;

namespace Bookings.Infrastructure.Services.IntegrationEvents
{
    public class EventMessage
    {
        public EventMessage(IIntegrationEvent integrationEvent)
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            IntegrationEvent = integrationEvent;
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public IIntegrationEvent IntegrationEvent { get; }
    }
}