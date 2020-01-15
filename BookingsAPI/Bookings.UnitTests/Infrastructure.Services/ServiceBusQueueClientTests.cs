using Bookings.Infrastructure.Services.ServiceBusQueue;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Bookings.UnitTests.Infrastructure.Services
{
    public class ServiceBusQueueClientTests
    {
        private ServiceBusQueueClient _client;
        
        [SetUp]
        public void Setup()
        {
            var settings = new ServiceBusSettings()
            {
                ConnectionString = "DevelopmentMode=true",
                QueueName = "booking"
            };
            _client = new ServiceBusQueueClient(Options.Create<ServiceBusSettings>(settings));
        }

        [Test]
        public void should_initialise_serialiser_settings()
        {
            _client.SerializerSettings.Should().BeOfType<JsonSerializerSettings>();
        }
    }
}