using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Infrastructure.Services
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
            individual1.HearingRole = new HearingRole(1, "Litigant in person") {UserRole = new UserRole(1, "Individual")};
            individual1.CaseRole = new CaseRole(1, "test");

            var participantAddedIntegrationEvent =
                new ParticipantsAddedIntegrationEvent(hearing, new List<Participant> {individual1});
            _eventPublisher.PublishAsync(participantAddedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<ParticipantsAddedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_ParticipantRemovedIntegrationEvent_is_raised()
        {
            var participantRemovedIntegrationEvent =
                new ParticipantRemovedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid());
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
            individual1.HearingRole = new HearingRole(1, "Litigant in person") {UserRole = new UserRole(1, "Individual")};
            individual1.CaseRole = new CaseRole(1, "test");

            var participantUpdatedIntegrationEvent =
                new ParticipantUpdatedIntegrationEvent(Guid.NewGuid(), individual1);
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
            individual1.CaseRole = new CaseRole(1, "test");

            var individual2 = individuals.Last();
            individual2.CaseRole = new CaseRole(2, "test2");

            var representative = hearing.GetParticipants().Single(x => x is Representative);
            representative.CaseRole = new CaseRole(3, "test3");

            var judge = hearing.GetParticipants().Single(x => x is Judge);
            judge.CaseRole = new CaseRole(3, "test4");

            var joh = hearing.GetParticipants().Single(x => x is JudicialOfficeHolder);
            joh.CaseRole = new CaseRole(4, "test5");
            var staffMember = hearing.GetParticipants().Single(x => x is StaffMember);
            staffMember.CaseRole = new CaseRole(5, "test5");

            hearing.AddEndpoints(new List<Endpoint>
            {
                new Endpoint("one", Guid.NewGuid().ToString(), "1234", null),
                new Endpoint("two", Guid.NewGuid().ToString(), "1234", representative)
            });

            var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing, hearing.Participants);
            _eventPublisher.PublishAsync(hearingIsReadyForVideoIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
            var typedEvent = (HearingIsReadyForVideoIntegrationEvent) @event.IntegrationEvent;
            typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
            typedEvent.Participants.Count.Should().Be(hearing.GetParticipants().Count);
            typedEvent.Endpoints.Should().NotBeNull();
            typedEvent.Endpoints.Count.Should().Be(hearing.GetEndpoints().Count);
        }

        [Test]
        public void Should_publish_message_to_queue_when_HearingIsReadyForVideoIntegrationEvent_with_otherinformation_is_raised()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);
            var email = "judge.login@hearings.reform.hmcts.net";
            var phone = "9594395734534";
            hearing.OtherInformation = $"JudgeEmail|{email}|JudgePhone|{phone}";
            var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();

            var individual1 = individuals.First();
            individual1.CaseRole = new CaseRole(1, "test");

            var individual2 = individuals.Last();
            individual2.CaseRole = new CaseRole(2, "test2");

            var representative = hearing.GetParticipants().Single(x => x is Representative);
            representative.CaseRole = new CaseRole(3, "test3");

            var judge = hearing.GetParticipants().Single(x => x is Judge);
            judge.CaseRole = new CaseRole(3, "test4");
            
            var joh = hearing.GetParticipants().Single(x => x is JudicialOfficeHolder);
            joh.CaseRole = new CaseRole(4, "test5");
            var staffMember = hearing.GetParticipants().Single(x => x is StaffMember);
            staffMember.CaseRole = new CaseRole(5, "test5");

            hearing.AddEndpoints(new List<Endpoint>
            {
                new Endpoint("one", Guid.NewGuid().ToString(), "1234", null),
                new Endpoint("two", Guid.NewGuid().ToString(), "1234", representative)
            });

            var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing, hearing.Participants);
            _eventPublisher.PublishAsync(hearingIsReadyForVideoIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
            var typedEvent = (HearingIsReadyForVideoIntegrationEvent)@event.IntegrationEvent;
            typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
            typedEvent.Participants.Count.Should().Be(hearing.GetParticipants().Count);
            typedEvent.Endpoints.Should().NotBeNull();
            typedEvent.Endpoints.Count.Should().Be(hearing.GetEndpoints().Count);
            var sentJudge = typedEvent.Participants.Single(x => x.UserRole == "Judge");
            sentJudge.ContactEmailForNonEJudJudgeUser.Should().Be(email);
            sentJudge.ContactPhoneForNonEJudJudgeUser.Should().Be(phone);
            var participant1 = typedEvent.Participants.First(x => x.UserRole != "Judge");
            participant1.ContactEmailForNonEJudJudgeUser.Should().BeNull();
            participant1.ContactPhoneForNonEJudJudgeUser.Should().BeNull();
        }

        [Test]
        public void Should_publish_message_to_queue_when_EndpointAddedIntegrationEvent_is_raised()
        {
            var endpointAddedIntegrationEvent =
                new EndpointAddedIntegrationEvent(Guid.NewGuid(), new Endpoint("one", "sip", "1234", null));
            _eventPublisher.PublishAsync(endpointAddedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<EndpointAddedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_EndpointAddedIntegrationEvent_is_raised_with_defence_advocate()
        {
            var dA = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var endpointAddedIntegrationEvent =
                new EndpointAddedIntegrationEvent(Guid.NewGuid(), new Endpoint("one", "sip", "1234", dA));
            _eventPublisher.PublishAsync(endpointAddedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<EndpointAddedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_EndpointRemovedIntegrationEvent_is_raised()
        {
            var endpointRemovedIntegrationEvent = new EndpointRemovedIntegrationEvent(Guid.NewGuid(), "sip");
            _eventPublisher.PublishAsync(endpointRemovedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<EndpointRemovedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_EndpointUpdatedIntegrationEvent_is_raised()
        {
            var endpointUpdatedIntegrationEvent =
                new EndpointUpdatedIntegrationEvent(Guid.NewGuid(), "sip", "name", "sol1@hmcts.net");
            _eventPublisher.PublishAsync(endpointUpdatedIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<EndpointUpdatedIntegrationEvent>();
        }

        [Test]
        public void Should_publish_message_to_queue_when_HearingIsReadyForVideoIntegrationEvent_is_raised_with_null_OtherInformation()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.OtherInformation = null;
            hearing.CaseType = new CaseType(1, "test");
            hearing.AddCase("1234", "test", true);

            var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing, hearing.Participants);
            _eventPublisher.PublishAsync(hearingIsReadyForVideoIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
            var typedEvent = (HearingIsReadyForVideoIntegrationEvent)@event.IntegrationEvent;
            typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
            typedEvent.Participants.Count.Should().Be(hearing.GetParticipants().Count);
        }
        
        [Test]
        public void Should_publish_message_to_queue_when_AllocationHearingsIntegrationEvent_is_raised_with_list_of_hearings()
        {
            List<VideoHearing> hearings;
            hearings = new List<VideoHearing>
            {
                CreateHearingWithCase(),
                CreateHearingWithCase(),
                CreateHearingWithCase(),
                CreateHearingWithCase(),
                CreateHearingWithCase()
            };

            var csoUser = new JusticeUser()
            {
                Username = "userName",
                UserRole = new UserRole(1,"VHO")
            };
            

            var allocationHearingsIntegrationEvent = new AllocationHearingsIntegrationEvent(hearings, csoUser);
            _eventPublisher.PublishAsync(allocationHearingsIntegrationEvent);

            _serviceBusQueueClient.Count.Should().Be(1);
            var @event = _serviceBusQueueClient.ReadMessageFromQueue();
            @event.IntegrationEvent.Should().BeOfType<AllocationHearingsIntegrationEvent>();
            var typedEvent = (AllocationHearingsIntegrationEvent)@event.IntegrationEvent;
            typedEvent.Hearings.Should().NotContainNulls();
            typedEvent.AllocatedCso.Should().NotBeNull();
        }
        
        private VideoHearing CreateHearingWithCase()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("1", "test case", true);
            return hearing;
        }
    }
}