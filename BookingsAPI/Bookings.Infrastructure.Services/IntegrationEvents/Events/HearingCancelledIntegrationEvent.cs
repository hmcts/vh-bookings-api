using System;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingCancelledIntegrationEvent : IIntegrationEvent
    {
        public HearingCancelledIntegrationEvent(Guid hearingId)
        {
            HearingId = hearingId;
        }

        public Guid HearingId { get; }
        public IntegrationEventType EventType { get; } = IntegrationEventType.HearingCancelled;
    }
}