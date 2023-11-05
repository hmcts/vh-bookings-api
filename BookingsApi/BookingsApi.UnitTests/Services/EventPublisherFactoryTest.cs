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

            var publishers = new List<IPublishEvent>
            {
                new WelcomeEmailForNewParticipantsPublisher(_eventPublisher),
                new CreateConferencePublisher(_eventPublisher),
                new HearingConfirmationforNewParticipantsPublisher(_eventPublisher),
                new HearingConfirmationforExistingParticipantsPublisher(_eventPublisher),
                new MultidayHearingConfirmationforNewParticipantsPublisher(_eventPublisher),
                new MultidayHearingConfirmationforExistingParticipantsPublisher(_eventPublisher)
            };

            var eventPublisherFactory = new EventPublisherFactory(publishers);
            eventPublisherFactory.Get(EventType.WelcomeMessageForNewParticipantEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.CreateConferenceEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.HearingConfirmationForNewParticipantEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.HearingConfirmationForExistingParticipantEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.MultidayHearingConfirmationforExistingParticipantEvent).Should().NotBeNull();
            eventPublisherFactory.Get(EventType.MultidayHearingConfirmationforNewParticipantEvent).Should().NotBeNull();
        }
    }
}