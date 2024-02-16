using System.Collections.Generic;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.Dtos;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Testing.Common.Stubs;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class ClonedMultidaysAsynchronousProcessTests
    {
        private readonly ClonedMultidaysAsynchronousProcess _clonedMultidaysAsynchronousProcess;
        private readonly IEventPublisher _eventPublisher;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;
        private readonly IEventPublisherFactory _eventPublisherFactory;
        private readonly IFeatureToggles _featureToggles;
        public ClonedMultidaysAsynchronousProcessTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);
            _eventPublisherFactory = EventPublisherFactoryInstance.Get(_eventPublisher);
            _featureToggles = new FeatureTogglesStub();
            _clonedMultidaysAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(_eventPublisherFactory, _featureToggles);
        }
        
        [Test]
        public async Task Should_publish_messages_with_new_notify_templates_feature_toggled_on()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.Participants[0].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[0].Person, 
                hearing.Participants[0].Person.CreatedDate.AddDays(-10), null);
            hearing.Participants[1].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[1].Person,
                hearing.Participants[1].Person.CreatedDate.AddDays(-10), null);

            ((FeatureTogglesStub)_featureToggles).NewTemplates = true;
            
            var judgeAsExistingParticipant = 1;
            var createConfereceMessageCount = 0;
            var newParticipantWelcomeMessageCount = 0;
            var multidayHearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count(x => x is not Judge) - 2;
            var mulitdayHearingConfirmationForExistingParticipantsMessageCount = 2;
            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + multidayHearingConfirmationForNewParticipantsMessageCount
                + mulitdayHearingConfirmationForExistingParticipantsMessageCount + judgeAsExistingParticipant;

            await _clonedMultidaysAsynchronousProcess.Start(hearing, 2);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent).
                Should().Be(multidayHearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent).
                Should().Be(mulitdayHearingConfirmationForExistingParticipantsMessageCount + judgeAsExistingParticipant);
        }

        [Test]
        public async Task Should_publish_messages_with_new_notify_templates_feature_toggled_off()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.Participants[0].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[0].Person, 
                hearing.Participants[0].Person.CreatedDate.AddDays(-10), null);
            hearing.Participants[1].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[1].Person,
                hearing.Participants[1].Person.CreatedDate.AddDays(-10), null);

            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;

            var expectedTotalMessageCount = 6;
            var totalDays = 2;
            
            // Act
            await _clonedMultidaysAsynchronousProcess.Start(hearing, totalDays);
            
            // Assert
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(expectedTotalMessageCount);
            
            messages.Count(x => x.IntegrationEvent is MultiDayHearingIntegrationEvent).Should().Be(expectedTotalMessageCount);
            var hearingConfirmationDtos = GetHearingConfirmationDtos(hearing);
            var multiDayIntegrationEvents = messages
                .Where(x => x.IntegrationEvent is MultiDayHearingIntegrationEvent)
                .Select(x => x.IntegrationEvent as MultiDayHearingIntegrationEvent)
                .ToList();
            multiDayIntegrationEvents.TrueForAll(x => x.TotalDays == totalDays).Should().BeTrue();
            multiDayIntegrationEvents.Select(x => x.HearingConfirmationForParticipant)
                .Should().BeEquivalentTo(hearingConfirmationDtos);
        }

        private static IEnumerable<HearingConfirmationForParticipantDto> GetHearingConfirmationDtos(Hearing hearing)
        {
            var participantDtos = hearing.Participants
                .Select(p => ParticipantDtoMapper.MapToDto(p, hearing.OtherInformation))
                .ToList();
            var @case = hearing.GetCases()[0];
            var hearingConfirmationDtos = participantDtos
                .Select(p => EventDtoMappers.MapToHearingConfirmationDto(
                    hearing.Id, hearing.ScheduledDateTime, p, @case))
                .ToList();

            return hearingConfirmationDtos;
        }
    }
}