using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Newtonsoft.Json;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        private readonly ConcurrentQueue<EventMessage> _eventMessages = new();

        public Task PublishMessageAsync(EventMessage eventMessage)
        {
            _eventMessages.Enqueue(eventMessage);
            return Task.CompletedTask;
        }

        public EventMessage ReadMessageFromQueue()
        {
            _eventMessages.TryDequeue(out var message);
            return message;
        }
        
        public EventMessage[] ReadAllMessagesFromQueue(Guid hearingId)
        {
            var list = (from message in _eventMessages
                        where JsonConvert.SerializeObject(message).Contains(hearingId.ToString())
                        select message).ToList();
            return list.ToArray();
        }
        
        public void ClearMessages()
        {
            _eventMessages.Clear();
        }
        
        public int Count => _eventMessages.Count;
    }
}