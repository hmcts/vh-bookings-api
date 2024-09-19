using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BookingsApi.Infrastructure.Services.IntegrationEvents;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        private JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // Or customize as needed
            WriteIndented = true  // Optional: formats the JSON for readability
        };

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
                where JsonSerializer.Serialize(message, SerializerOptions).Contains(hearingId.ToString())
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