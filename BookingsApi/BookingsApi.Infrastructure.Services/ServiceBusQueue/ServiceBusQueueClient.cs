using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BookingsApi.Common.Helpers;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    public interface IServiceBusQueueClient
    {
        Task PublishMessageAsync(EventMessage eventMessage);
    }

    [ExcludeFromCodeCoverage]
    public class ServiceBusQueueClient(IOptions<ServiceBusSettings> serviceBusSettings) : IServiceBusQueueClient
    {
        private readonly ServiceBusSettings _serviceBusSettings = serviceBusSettings.Value;
        public JsonSerializerSettings SerializerSettings { get; } = DefaultSerializerSettings.DefaultNewtonsoftSerializerSettings();

        public async Task PublishMessageAsync(EventMessage eventMessage)
        {
            await using var client = new ServiceBusClient(_serviceBusSettings.ConnectionString);
            var sender = client.CreateSender(_serviceBusSettings.QueueName); 
            var jsonObjectString = JsonConvert.SerializeObject(eventMessage, SerializerSettings);
            
            var messageBytes = Encoding.UTF8.GetBytes(jsonObjectString);
            await sender.SendMessageAsync(new ServiceBusMessage(messageBytes)).ConfigureAwait(false);
        }
    }
}