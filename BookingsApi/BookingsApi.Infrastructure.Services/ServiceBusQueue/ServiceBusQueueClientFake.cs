using System.Collections.Concurrent;
using System.Threading.Tasks;
using BookingsApi.Common.Helpers;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Newtonsoft.Json;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        public JsonSerializerSettings SerializerSettings { get; set; }
        private readonly ConcurrentQueue<EventMessage> _eventMessages = new ConcurrentQueue<EventMessage>();

        public ServiceBusQueueClientFake()
        {
            SerializerSettings = DefaultSerializerSettings.DefaultNewtonsoftSerializerSettings();
        }
        
        public Task PublishMessageAsync(EventMessage eventMessage)
        {
            var jsonObjectString = JsonConvert.SerializeObject(eventMessage, SerializerSettings);
            _eventMessages.Enqueue(eventMessage);
            return Task.CompletedTask;
        }

        public EventMessage ReadMessageFromQueue()
        {
            _eventMessages.TryDequeue(out var message);
            return message;
        }
        public EventMessage[] ReadAllMessagesFromQueue()
        {
            var messages = _eventMessages.ToArray();
            _eventMessages.Clear();
            return messages;
        }
        public int Count => _eventMessages.Count;
    }
}