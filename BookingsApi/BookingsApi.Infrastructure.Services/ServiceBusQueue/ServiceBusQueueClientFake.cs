using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                        where JsonConvert.SerializeObject(message, SerializerSettings).Contains(hearingId.ToString())
                        select message).ToList();
            return list.ToArray();
        }
        public int Count => _eventMessages.Count;
    }
}