﻿using System.Text;
using System.Threading.Tasks;
using BookingsApi.Common.Helpers;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    public interface IServiceBusQueueClient
    {
        Task PublishMessageAsync(EventMessage eventMessage);
    }

    public class ServiceBusQueueClient : IServiceBusQueueClient
    {
        private readonly ServiceBusSettings _serviceBusSettings;
        public JsonSerializerSettings SerializerSettings { get; set; }

        public ServiceBusQueueClient(IOptions<ServiceBusSettings> serviceBusSettings)
        {
            _serviceBusSettings = serviceBusSettings.Value;
            SerializerSettings = DefaultSerializerSettings.DefaultNewtonsoftSerializerSettings();
        }

        public async Task PublishMessageAsync(EventMessage eventMessage)
        {
            var queueClient = new QueueClient(_serviceBusSettings.ConnectionString, _serviceBusSettings.QueueName);
            var jsonObjectString = JsonConvert.SerializeObject(eventMessage, SerializerSettings);
            
            var messageBytes = Encoding.UTF8.GetBytes(jsonObjectString);
            await queueClient.SendAsync(new Message(messageBytes)).ConfigureAwait(false);
        }
    }
}