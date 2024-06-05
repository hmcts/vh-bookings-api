using System;
using System.Collections.Generic;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointUpdatedIntegrationEvent(Guid hearingId, string sip, string displayName) : IIntegrationEvent
    {
        public Guid HearingId { get; } = hearingId;
        public string Sip { get; } = sip;
        public string DisplayName { get; } = displayName;

        public List<string> EndpointParticipants { get; set; }
        public List<string> EndpointParticipantsAdded { get; set; }
        public List<string> EndpointParticipantsRemoved { get; set; }
    }
}