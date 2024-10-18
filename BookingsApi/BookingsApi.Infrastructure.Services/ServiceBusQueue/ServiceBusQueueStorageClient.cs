using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using BookingsApi.Common.Helpers;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue;

public class ServiceBusQueueStorageClient : IServiceBusQueueClient
{
    private readonly QueueClient _queueClient;
    public JsonSerializerSettings SerializerSettings { get; } = DefaultSerializerSettings.DefaultNewtonsoftSerializerSettings();

    public ServiceBusQueueStorageClient(IOptions<ServiceBusSettings> serviceBusSettings)
    {
        var settings = serviceBusSettings.Value;
        _queueClient = new QueueClient(settings.ConnectionString, settings.QueueName);
    }

    public async Task PublishMessageAsync(EventMessage eventMessage)
    {
        var jsonObjectString = JsonConvert.SerializeObject(eventMessage, SerializerSettings);
        var messageBytes = Encoding.UTF8.GetBytes(jsonObjectString);
        var base64Message = Convert.ToBase64String(messageBytes);

        await _queueClient.SendMessageAsync(base64Message);
    }
}