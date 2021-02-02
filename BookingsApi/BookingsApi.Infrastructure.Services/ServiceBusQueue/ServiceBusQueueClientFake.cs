using System.Collections.Concurrent;
using System.Threading.Tasks;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Bookings.Infrastructure.Services.ServiceBusQueue
{
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        public JsonSerializerSettings SerializerSettings { get; set; }
        private readonly ConcurrentQueue<EventMessage> _eventMessages = new ConcurrentQueue<EventMessage>();

        public ServiceBusQueueClientFake()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented,
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

        public int Count => _eventMessages.Count;
    }
}