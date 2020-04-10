using System;
using System.Collections.Generic;
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
        public void Should_publish_message_to_queue_when_HearingCancelledIntegrationEvent_is_raised()
        {
            var hearingCancelledEvent = new HearingCancelledIntegrationEvent(Guid.NewGuid());
            _eventPublisher.PublishAsync(hearingCancelledEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingCancelledIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_HearingDetailsUpdatedIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);

            var hearingDetailsUpdatedIntegrationEvent = new HearingDetailsUpdatedIntegrationEvent(hearing);
            _eventPublisher.PublishAsync(hearingDetailsUpdatedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingDetailsUpdatedIntegrationEvent>();
            
            var typedEvent = (HearingDetailsUpdatedIntegrationEvent) @event.IntegrationEvent;
            typedEvent.Hearing.CaseName.Should().Be(hearing.GetCases().First().Name);
            typedEvent.Hearing.CaseNumber.Should().Be(hearing.GetCases().First().Number);
            typedEvent.Hearing.CaseType.Should().Be(hearing.CaseType.Name);
            typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
        }

        [Test]
        public void Should_publish_message_to_queue_when_ParticipantsAddedIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);
            var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();

            var individual1 = individuals.First();
            individual1.HearingRole = new HearingRole(1, "Claimant LIP") { UserRole = new UserRole(1, "Individual") };
            individual1.CaseRole = new CaseRole(1, "test");

            var participantAddedIntegrationEvent = new ParticipantsAddedIntegrationEvent(hearing.Id, new List<Participant>{ individual1});
            _eventPublisher.PublishAsync(participantAddedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<ParticipantsAddedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_ParticipantRemovedIntegrationEvent_is_raised()
        {
            var participantRemovedIntegrationEvent = new ParticipantRemovedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid());
            _eventPublisher.PublishAsync(participantRemovedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<ParticipantRemovedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_ParticipantUpdatedIntegrationEvent_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();
            var individual1 = individuals.First();
            individual1.HearingRole = new HearingRole(1, "Claimant LIP") { UserRole = new UserRole(1, "Individual") };
            individual1.CaseRole = new CaseRole(1, "test");

            var participantUpdatedIntegrationEvent = new ParticipantUpdatedIntegrationEvent(Guid.NewGuid(), individual1);
            _eventPublisher.PublishAsync(participantUpdatedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_HearingIsReadyForVideoIntegrationEvent_is_raised()
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
            representative.HearingRole = new HearingRole(5, "Representative"){ UserRole = new UserRole(2, "Representative") } ;
            representative.CaseRole= new CaseRole(3, "test3");

            var judge = hearing.GetParticipants().Single(x => x is Judge);
            judge.HearingRole = new HearingRole(5, "Judge") { UserRole = new UserRole(2, "Judge") };
            judge.CaseRole = new CaseRole(3, "test4");
            
            var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing);
            _eventPublisher.PublishAsync(hearingIsReadyForVideoIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
            var typedEvent = (HearingIsReadyForVideoIntegrationEvent) @event.IntegrationEvent;
            typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
            typedEvent.Participants.Count.Should().Be(hearing.GetParticipants().Count);
        }
    }
}