using System;
using System.Linq;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Infrastructure.Services
{
    public class IntegrationEventTests
    {
        private ServiceBusQueueClientFake _serviceBusQueueClient;
        private IEventPublisher _eventPublisher;

        [SetUp]
        public void Setup()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);
        }

        [Test]
        public void should_publish_message_to_queue_when_HearingCancelledIntegrationEvent_is_raised()
        {
            var hearingCancelledEvent = new HearingCancelledIntegrationEvent(Guid.NewGuid());
            _eventPublisher.PublishAsync(hearingCancelledEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingCancelledIntegrationEvent>();
            @event.IntegrationEvent.EventType.Should().Be(IntegrationEventType.HearingCancelled);
        }

        [Test]
        public void should_publish_message_to_queue_when_HearingDetailsUpdatedIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);

            var hearingDetailsUpdatedIntegrationEvent = new HearingDetailsUpdatedIntegrationEvent(hearing);
            _eventPublisher.PublishAsync(hearingDetailsUpdatedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingDetailsUpdatedIntegrationEvent>();
            @event.IntegrationEvent.EventType.Should().Be(IntegrationEventType.HearingDetailsUpdated);
        }

        [Test]
        public void should_publish_message_to_queue_when_ParticipantAddedIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);
            var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();

            var individual1 = individuals.First();
            individual1.HearingRole = new HearingRole(1, "Claimant LIP") { UserRole = new UserRole(1, "Individual") };
            individual1.CaseRole = new CaseRole(1, "test");

            var participantAddedIntegrationEvent = new ParticipantAddedIntegrationEvent(hearing.Id, individual1);
            _eventPublisher.PublishAsync(participantAddedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<ParticipantAddedIntegrationEvent>();
            @event.IntegrationEvent.EventType.Should().Be(IntegrationEventType.ParticipantAdded);
        }

        [Test]
        public void should_publish_message_to_queue_when_ParticipantRemovedIntegrationEvent_is_raised()
        {
            var participantRemovedIntegrationEvent = new ParticipantRemovedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid());
            _eventPublisher.PublishAsync(participantRemovedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<ParticipantRemovedIntegrationEvent>();
            @event.IntegrationEvent.EventType.Should().Be(IntegrationEventType.ParticipantRemoved);
        }

        [Test]
        public void should_publish_message_to_queue_when_HearingIsReadyForVideoIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);
            var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();

            var individual1 = individuals.First();
            individual1.HearingRole = new HearingRole(1, "Claimant LIP") { UserRole = new UserRole(1, "Individual") };
            individual1.CaseRole = new CaseRole(1, "test");

            var individual2 = individuals.Last();
            individual2.HearingRole = new HearingRole(2, "Defendant LIP") { UserRole = new UserRole(1, "Individual") };
            individual2.CaseRole = new CaseRole(2, "test2");

            var representative = hearing.GetParticipants().Single(x => x is Representative);
            representative.HearingRole = new HearingRole(5, "Solicitor"){ UserRole = new UserRole(2, "Solitcitor") } ;
            representative.CaseRole= new CaseRole(3, "test3");

            var judge = hearing.GetParticipants().Single(x => x is Judge);
            judge.HearingRole = new HearingRole(5, "Judge") { UserRole = new UserRole(2, "Judge") };
            judge.CaseRole = new CaseRole(3, "test4");
            
            var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing);
            _eventPublisher.PublishAsync(hearingIsReadyForVideoIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
            @event.IntegrationEvent.EventType.Should().Be(IntegrationEventType.HearingIsReadyForVideo);
        }
    }
}