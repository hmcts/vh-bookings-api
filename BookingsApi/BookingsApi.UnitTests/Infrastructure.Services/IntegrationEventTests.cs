using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services.Dtos;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.UnitTests.Infrastructure.Services;

public class IntegrationEventTests
{
    private ServiceBusQueueClientFake _serviceBusQueueClient;
    private EventPublisher _eventPublisher;

    [SetUp]
    public void Setup()
    {
        _serviceBusQueueClient = new ServiceBusQueueClientFake();
        _eventPublisher = new EventPublisher(_serviceBusQueueClient);
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_HearingCancelledIntegrationEvent_is_raised()
    {
        var hearingCancelledEvent = new HearingCancelledIntegrationEvent(Guid.NewGuid());
        await _eventPublisher.PublishAsync(hearingCancelledEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<HearingCancelledIntegrationEvent>();
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_HearingDetailsUpdatedIntegrationEvent_is_raised()
    {
        var hearing = new VideoHearingBuilder().Build();
        hearing.CaseType = new CaseType(1, "test");
        hearing.AddCase("1234", "test", true);

        var hearingDetailsUpdatedIntegrationEvent = new HearingDetailsUpdatedIntegrationEvent(hearing);
        await _eventPublisher.PublishAsync(hearingDetailsUpdatedIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<HearingDetailsUpdatedIntegrationEvent>();

        var typedEvent = (HearingDetailsUpdatedIntegrationEvent) @event.IntegrationEvent;
        typedEvent.Hearing.CaseName.Should().Be(hearing.GetCases()[0].Name);
        typedEvent.Hearing.CaseNumber.Should().Be(hearing.GetCases()[0].Number);
        typedEvent.Hearing.CaseType.Should().Be(hearing.CaseType.Name);
        typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_ParticipantsAddedIntegrationEvent_is_raised()
    {
        var hearing = new VideoHearingBuilder().Build();
        hearing.CaseType = new CaseType(1, "test");
        hearing.AddCase("1234", "test", true);
        var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();

        var individual1 = individuals[0];
        individual1.HearingRole = new HearingRole(1, "Litigant in person") {UserRole = new UserRole(1, "Individual")};

        var participantAddedIntegrationEvent =
            new ParticipantsAddedIntegrationEvent(hearing, new List<Participant> {individual1});
        await _eventPublisher.PublishAsync(participantAddedIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<ParticipantsAddedIntegrationEvent>();
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_ParticipantRemovedIntegrationEvent_is_raised()
    {
        var participantRemovedIntegrationEvent =
            new ParticipantRemovedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid());
        await _eventPublisher.PublishAsync(participantRemovedIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<ParticipantRemovedIntegrationEvent>();
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_ParticipantUpdatedIntegrationEvent_is_raised()
    {
        var hearing = new VideoHearingBuilder().Build();
        var individuals = hearing.GetParticipants().Where(x => x is Individual).ToList();
        var individual1 = individuals[0];
        individual1.HearingRole = new HearingRole(1, "Litigant in person") {UserRole = new UserRole(1, "Individual")};
        
        var participantUpdatedIntegrationEvent =
            new ParticipantUpdatedIntegrationEvent(Guid.NewGuid(), individual1);
        await _eventPublisher.PublishAsync(participantUpdatedIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<ParticipantUpdatedIntegrationEvent>();
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_HearingIsReadyForVideoIntegrationEvent_is_raised()
    {
        var hearing = new VideoHearingBuilder(addJudge:false).Build();
        hearing.CaseType = new CaseType(1, "test");
        hearing.AddCase("1234", "test", true);

        var representative = hearing.GetParticipants().Single(x => x is Representative);

        var judgePerson = new JudiciaryPersonBuilder("Judge123").Build();
        var johPerson = new JudiciaryPersonBuilder("Joh123").Build();

        hearing.AddJudiciaryJudge(judgePerson, "Judge");
        hearing.AddJudiciaryPanelMember(johPerson, "Joh");
            
        hearing.AddEndpoints(new List<Endpoint>
        {
            new Endpoint(Guid.NewGuid().ToString(),"one", Guid.NewGuid().ToString(), "1234", null),
            new Endpoint(Guid.NewGuid().ToString(),"two", Guid.NewGuid().ToString(), "1234", representative)
        });

        var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing, hearing.Participants);
        await _eventPublisher.PublishAsync(hearingIsReadyForVideoIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
        var typedEvent = (HearingIsReadyForVideoIntegrationEvent) @event.IntegrationEvent;
        typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
        typedEvent.Participants.Count.Should().Be(hearing.GetParticipants().Count + hearing.GetJudiciaryParticipants().Count);
        typedEvent.Endpoints.Should().NotBeNull();
        typedEvent.Endpoints.Count.Should().Be(hearing.GetEndpoints().Count);
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_EndpointAddedIntegrationEvent_is_raised()
    {
        var hearing = new VideoHearingBuilder().Build();
        hearing.AddEndpoint(new Endpoint(Guid.NewGuid().ToString(), "one", "sip", "1234", null));
        var endpointAddedIntegrationEvent =
            new EndpointAddedIntegrationEvent(hearing, hearing.GetEndpoints().First());
        await _eventPublisher.PublishAsync(endpointAddedIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<EndpointAddedIntegrationEvent>();
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_EndpointAddedIntegrationEvent_is_raised_with_defence_advocate()
    {
        var hearing = new VideoHearingBuilder().Build();
        var dA = hearing.GetParticipants().First(x => x is Representative);
        hearing.AddEndpoint(new Endpoint(Guid.NewGuid().ToString(), "one", "sip", "1234", dA));
        var endpointAddedIntegrationEvent =
            new EndpointAddedIntegrationEvent(hearing, hearing.GetEndpoints().First());
        await _eventPublisher.PublishAsync(endpointAddedIntegrationEvent);

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
    public async Task Should_publish_message_to_queue_when_EndpointUpdatedIntegrationEvent_is_raised()
    {
        var endpointUpdatedIntegrationEvent =
            new EndpointUpdatedIntegrationEvent(Guid.NewGuid(), "sip", "name", "sol1@hmcts.net", ConferenceRole.Host);
        await _eventPublisher.PublishAsync(endpointUpdatedIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<EndpointUpdatedIntegrationEvent>();
    }

    [Test]
    public async Task Should_publish_message_to_queue_when_HearingIsReadyForVideoIntegrationEvent_is_raised_with_null_OtherInformation()
    {
        var hearing = new VideoHearingBuilder().Build();
        hearing.OtherInformation = null;
        hearing.CaseType = new CaseType(1, "test");
        hearing.AddCase("1234", "test", true);

        var hearingIsReadyForVideoIntegrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing, hearing.Participants);
        await _eventPublisher.PublishAsync(hearingIsReadyForVideoIntegrationEvent);

        _serviceBusQueueClient.Count.Should().Be(1);
        var @event = _serviceBusQueueClient.ReadMessageFromQueue();
        @event.IntegrationEvent.Should().BeOfType<HearingIsReadyForVideoIntegrationEvent>();
        var typedEvent = (HearingIsReadyForVideoIntegrationEvent)@event.IntegrationEvent;
        typedEvent.Hearing.RecordAudio.Should().Be(hearing.AudioRecordingRequired);
        typedEvent.Participants.Count.Should().Be(hearing.GetParticipants().Count + hearing.GetJudiciaryParticipants().Count);
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
            Username = "userName"
        };
            
        csoUser.JusticeUserRoles = new List<JusticeUserRole>
        {
            new() { UserRole = new UserRole((int)UserRoleId.Vho, "Video Hearings Team Lead") }
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
        
    private static VideoHearing CreateHearingWithCase()
    {
        var hearing = new VideoHearingBuilder().Build();
        hearing.AddCase("1", "test case", true);
        return hearing;
    }
}