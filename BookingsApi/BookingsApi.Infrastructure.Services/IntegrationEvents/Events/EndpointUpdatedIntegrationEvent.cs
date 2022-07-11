using System;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public EndpointUpdatedIntegrationEvent(Guid hearingId, string sip, string displayName,
            string defenceAdvocateContactEmail)
        {
            HearingId = hearingId;
            Sip = sip;
            DisplayName = displayName;
            DefenceAdvocateContactEmail = defenceAdvocateContactEmail;
        }

        public Guid HearingId { get; }
        public string Sip { get; }
        public string DisplayName { get; }
        public string DefenceAdvocateContactEmail { get; }
    }
}