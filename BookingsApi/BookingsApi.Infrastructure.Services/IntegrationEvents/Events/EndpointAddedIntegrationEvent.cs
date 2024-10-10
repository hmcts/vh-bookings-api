using System;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class EndpointAddedIntegrationEvent : IIntegrationEvent
    {
        public EndpointAddedIntegrationEvent(Hearing hearing, Endpoint endpoint)
        {
            HearingId = hearing.Id;
            Endpoint = EndpointDtoMapper.MapToDto(endpoint, hearing.GetParticipants(), hearing.GetEndpoints());
        }

        public Guid HearingId { get; }
        public EndpointDto Endpoint { get; }
    }
}