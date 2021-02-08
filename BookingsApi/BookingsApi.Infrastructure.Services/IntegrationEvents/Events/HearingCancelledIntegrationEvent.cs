using System;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingCancelledIntegrationEvent : IIntegrationEvent
    {
        public HearingCancelledIntegrationEvent(Guid hearingId)
        {
            HearingId = hearingId;
        }

        public Guid HearingId { get; }
    }
}