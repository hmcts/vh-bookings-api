using Bookings.Infrastructure.Services.ServiceBusQueue;

namespace Bookings.Infrastructure.Services.IntegrationEvents.Events
{
    public interface IRaiseIntegrationEvent
    {
        void Raise(IIntegrationEvent integrationEvent);
    }

    public class RaiseIntegrationEvent : IRaiseIntegrationEvent
    {
        private readonly IServiceBusQueueClient _serviceBusQueueClient;

        public RaiseIntegrationEvent(IServiceBusQueueClient serviceBusQueueClient)
        {
            _serviceBusQueueClient = serviceBusQueueClient;
        }

        public void Raise(IIntegrationEvent integrationEvent)
        {
            _serviceBusQueueClient.PublishMessageAsync(integrationEvent);
        }
    }
}