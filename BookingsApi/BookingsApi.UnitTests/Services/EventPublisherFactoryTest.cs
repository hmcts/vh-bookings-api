﻿using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using System.Collections.Generic;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class EventPublisherFactoryTest
    {
        private IEventPublisher _eventPublisher;
        private ServiceBusQueueClientFake _serviceBusQueueClient;

        [Test]
        public void Should_return_all_publishers()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);

            var eventPublisherFactory = EventPublisherFactoryInstance.Get(_eventPublisher);
            eventPublisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.CreateConferenceEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.NewParticipantHearingConfirmationEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.ExistingParticipantHearingConfirmationEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.ExistingParticipantMultidayHearingConfirmationEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.NewParticipantMultidayHearingConfirmationEvent).Should().NotBeNull();
        }
    }
}