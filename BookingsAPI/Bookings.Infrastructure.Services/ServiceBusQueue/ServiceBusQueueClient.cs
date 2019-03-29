using System.Text;
using System.Threading.Tasks;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Bookings.Infrastructure.Services.ServiceBusQueue
{
    public interface IServiceBusQueueClient
    {
        Task PublishMessageAsync(IIntegrationEvent integrationEvent);
    }

    public class ServiceBusQueueClient : IServiceBusQueueClient
    {
        private readonly ServiceBusSettings _serviceBusSettings;

        public ServiceBusQueueClient(ServiceBusSettings serviceBusSettings)
        {
            _serviceBusSettings = serviceBusSettings;
        }

        public async Task PublishMessageAsync(IIntegrationEvent integrationEvent)
        {
            var queueClient = new QueueClient(_serviceBusSettings.ConnectionString, _serviceBusSettings.QueueName);
            var jsonObjectString = JsonConvert.SerializeObject(integrationEvent);

            var messageBytes = Encoding.UTF8.GetBytes(jsonObjectString);
            await queueClient.SendAsync(new Message(messageBytes));
        }
    }
}