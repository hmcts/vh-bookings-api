﻿using System.Threading.Tasks;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents
{
    public interface IEventPublisher
    {
        Task PublishAsync(IIntegrationEvent integrationEvent);
    }

    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceBusQueueClient _serviceBusQueueClient;

        public EventPublisher(IServiceBusQueueClient serviceBusQueueClient)
        {
            _serviceBusQueueClient = serviceBusQueueClient;
        }

        public async Task PublishAsync(IIntegrationEvent integrationEvent)
        {
            var message = new EventMessage(integrationEvent);
            await _serviceBusQueueClient.PublishMessageAsync(message);
        }
    }
}