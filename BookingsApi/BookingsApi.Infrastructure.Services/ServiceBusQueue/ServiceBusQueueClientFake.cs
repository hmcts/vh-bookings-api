using System.Collections.Concurrent;
using System.Threading.Tasks;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        public JsonSerializerSettings SerializerSettings { get; set; }
        private readonly ConcurrentQueue<EventMessage> _eventMessages = new();

        public ServiceBusQueueClientFake()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.Objects
            };
            SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        }
        
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
        public EventMessage[] ReadAllMessagesFromQueue()
        {
            return _eventMessages.ToArray();
        }

        public void Clear()
        {
            _eventMessages.Clear();
        }
        
        public int Count => _eventMessages.Count;
    }
}