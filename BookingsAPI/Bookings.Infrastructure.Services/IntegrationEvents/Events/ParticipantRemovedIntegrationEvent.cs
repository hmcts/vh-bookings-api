using System;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantRemovedIntegrationEvent : IIntegrationEvent
    {
        public ParticipantRemovedIntegrationEvent(Guid hearingId, Guid participantId)
        {
            HearingId = hearingId;
            ParticipantId = participantId;
        }

        public Guid HearingId { get; }
        public Guid ParticipantId { get; }

        public IntegrationEventType EventType { get; } = IntegrationEventType.ParticipantRemoved;
    }
}