using System;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public EndpointUpdatedIntegrationEvent(Guid hearingId, string sip, string displayName,
            string defenceAdvocateContactEmail, ConferenceRole role)
        {
            HearingId = hearingId;
            Sip = sip;
            DisplayName = displayName;
            DefenceAdvocate = defenceAdvocateContactEmail;
            Role = role;
        }
        
        public Guid HearingId { get; }
        public string Sip { get; }
        public string DisplayName { get; }
        public string DefenceAdvocate { get; }
        public ConferenceRole Role { get; }
    }
}