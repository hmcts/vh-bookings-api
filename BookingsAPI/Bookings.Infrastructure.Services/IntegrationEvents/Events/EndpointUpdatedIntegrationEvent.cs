using System;
using Bookings.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public EndpointUpdatedIntegrationEvent(Guid hearingId, Endpoint endpoint)
        {
            HearingId = hearingId;
            Endpoint = EndpointDtoMapper.MapToDto(endpoint);
        }
        
        public Guid HearingId { get; }
        public EndpointDto Endpoint { get; }
    }
}