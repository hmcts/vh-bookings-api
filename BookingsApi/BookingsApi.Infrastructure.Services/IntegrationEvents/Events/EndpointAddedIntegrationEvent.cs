using System;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
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