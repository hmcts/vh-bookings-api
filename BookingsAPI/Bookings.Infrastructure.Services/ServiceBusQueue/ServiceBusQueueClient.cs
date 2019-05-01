using System.Text;
using System.Threading.Tasks;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Bookings.Infrastructure.Services.ServiceBusQueue
{
    public interface IServiceBusQueueClient
    {
        Task PublishMessageAsync(IIntegrationEvent integrationEvent);
    }

    public class ServiceBusQueueClient : IServiceBusQueueClient
    {
        private readonly ServiceBusSettings _serviceBusSettings;
        private readonly JsonSerializerSettings _serializerSettings;

        public ServiceBusQueueClient(IOptions<ServiceBusSettings> serviceBusSettings)
        {
            _serviceBusSettings = serviceBusSettings.Value;

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented
            };
            _serializerSettings.Converters.Add(new StringEnumConverter(true));
        }

        public async Task PublishMessageAsync(IIntegrationEvent integrationEvent)
        {
            var queueClient = new QueueClient(_serviceBusSettings.ConnectionString, _serviceBusSettings.QueueName);
            var jsonObjectString = JsonConvert.SerializeObject(integrationEvent, _serializerSettings);

            var messageBytes = Encoding.UTF8.GetBytes(jsonObjectString);
            await queueClient.SendAsync(new Message(messageBytes)).ConfigureAwait(false);
        }
    }
}