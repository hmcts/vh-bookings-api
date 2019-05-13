using System.Linq;
using System.Threading.Tasks;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Infrastructure.Services.Dtos;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

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