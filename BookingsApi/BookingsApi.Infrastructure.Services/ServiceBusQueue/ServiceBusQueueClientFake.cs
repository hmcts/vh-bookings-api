using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Helpers;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Newtonsoft.Json;

namespace BookingsApi.Infrastructure.Services.ServiceBusQueue
{
    [ExcludeFromCodeCoverage(Justification = "This is a fake class used for testing purposes only")]
    public class ServiceBusQueueClientFake : IServiceBusQueueClient
    {
        private JsonSerializerSettings SerializerSettings { get; set; } = DefaultSerializerSettings.DefaultNewtonsoftSerializerSettings();
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
                        where JsonConvert.SerializeObject(message, SerializerSettings).Contains(hearingId.ToString())
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