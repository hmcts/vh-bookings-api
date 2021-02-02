using System;
using BookingsApi.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointAddedIntegrationEvent : IIntegrationEvent
    {
        public EndpointAddedIntegrationEvent(Guid hearingId, Endpoint endpoint)
        {
            HearingId = hearingId;
            Endpoint = EndpointDtoMapper.MapToDto(endpoint);
        }

        public Guid HearingId { get; }
        public EndpointDto Endpoint { get; }
    }
}