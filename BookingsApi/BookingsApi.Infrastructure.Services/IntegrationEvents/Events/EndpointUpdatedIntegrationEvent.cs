using System;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public EndpointUpdatedIntegrationEvent(Guid hearingId, string sip, string displayName,
            string defenceAdvocateUsername)
        {
            HearingId = hearingId;
            Sip = sip;
            DisplayName = displayName;
            DefenceAdvocateUsername = defenceAdvocateUsername;
        }

        public Guid HearingId { get; }
        public string Sip { get; }
        public string DisplayName { get; }
        public string DefenceAdvocateUsername { get; }
    }
}