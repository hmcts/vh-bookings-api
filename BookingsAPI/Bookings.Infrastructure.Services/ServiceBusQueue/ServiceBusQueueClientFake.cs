using System.Collections.Concurrent;
using System.Threading.Tasks;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;

namespace Bookings.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        private readonly ConcurrentQueue<IIntegrationEvent> _eventMessages = new ConcurrentQueue<IIntegrationEvent>();

        public Task PublishMessageAsync(IIntegrationEvent integrationEvent)
        {
            _eventMessages.Enqueue(integrationEvent);
            return Task.CompletedTask;
        }

        public IIntegrationEvent ReadMessageFromQueue()
        {
            _eventMessages.TryDequeue(out var message);
            return message;
        }

        public int Count => _eventMessages.Count;
    }
}