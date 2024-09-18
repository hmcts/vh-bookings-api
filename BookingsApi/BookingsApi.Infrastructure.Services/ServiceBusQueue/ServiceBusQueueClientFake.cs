using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BookingsApi.Common.Helpers;
using BookingsApi.Infrastructure.Services.IntegrationEvents;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        public JsonSerializerOptions SerializerSettings { get; } = DefaultSerializerSettings.DefaultSystemTextJsonSerializerSettings();
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
                        where JsonSerializer.Serialize(message, SerializerSettings).Contains(hearingId.ToString())
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