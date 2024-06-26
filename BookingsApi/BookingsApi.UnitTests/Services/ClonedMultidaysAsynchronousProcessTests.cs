using System.Collections.Generic;
using BookingsApi.Common;
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
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;
        private readonly IFeatureToggles _featureToggles;
        public ClonedMultidaysAsynchronousProcessTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            IEventPublisher eventPublisher = new EventPublisher(_serviceBusQueueClient);
            IEventPublisherFactory eventPublisherFactory = EventPublisherFactoryInstance.Get(eventPublisher);
            _featureToggles = new FeatureTogglesStub();
            _clonedMultidaysAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(eventPublisherFactory, _featureToggles);
        }
        
        [Test]
        public async Task Should_publish_messages_with_new_notify_templates_feature_on()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            // treat the first 2 participants an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(treatPersonAsNew: false).Build());
            hearing.Participants[1].ChangePerson(new PersonBuilder(treatPersonAsNew: false).Build());
            
            ((FeatureTogglesStub)_featureToggles).NewTemplates = true;
            
            var judgeAsExistingParticipant = 1;
            var createConfereceMessageCount = 0;
            var newParticipantWelcomeMessageCount = 0;
            var multidayHearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count(x => x is not Judge) - 2;
            var mulitdayHearingConfirmationForExistingParticipantsMessageCount = 2;
            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + multidayHearingConfirmationForNewParticipantsMessageCount
                + mulitdayHearingConfirmationForExistingParticipantsMessageCount + judgeAsExistingParticipant;
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            await _clonedMultidaysAsynchronousProcess.Start(hearing, 2, videoHearingUpdateDate);
            
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
        public async Task Should_publish_messages_for_v1_with_new_notify_templates_feature_off()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();

            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;

            const int expectedTotalMessageCount = 5;
            const int totalDays = 2;
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            // Act
            await _clonedMultidaysAsynchronousProcess.Start(hearing, totalDays, videoHearingUpdateDate);
            
            // Assert
            AssertEventsPublishedForNotifyFeatureOff(hearing, totalDays, expectedTotalMessageCount);
        }
        
        [Test]
        public async Task Should_publish_messages_with_v2_with_new_notify_templates_feature_off()
        {
            // Arrange
            var hearing = new VideoHearingBuilder(addJudge: false).WithCase().Build();
            var judiciaryJudgePerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
            hearing.AddJudiciaryJudge(judiciaryJudgePerson, "Judiciary Judge");
            var panelMemberParticipants = hearing.Participants.Where(p => p is JudicialOfficeHolder).ToList();
            foreach (var participant in panelMemberParticipants)
            {
                // Remove these since they are V1 johs and not applicable to V2
                hearing.Participants.Remove(participant);
            }

            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;

            const int expectedTotalMessageCount = 4;
            const int totalDays = 2;
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            // Act
            await _clonedMultidaysAsynchronousProcess.Start(hearing, totalDays, videoHearingUpdateDate);
            
            // Assert
            AssertEventsPublishedForNotifyFeatureOff(hearing, totalDays, expectedTotalMessageCount);
        }
        
        private void AssertEventsPublishedForNotifyFeatureOff(Hearing hearing, int totalDayCount, int expectedTotalMessageCount)
        {
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);

            messages.Count(x => x.IntegrationEvent is MultiDayHearingIntegrationEvent).Should().Be(expectedTotalMessageCount);
            var hearingConfirmationDtos = HearingConfirmationForParticipantDtoMapper.MapToDtos(hearing);
            var multiDayIntegrationEvents = messages
                .Where(x => x.IntegrationEvent is MultiDayHearingIntegrationEvent)
                .Select(x => x.IntegrationEvent as MultiDayHearingIntegrationEvent)
                .ToList();
            multiDayIntegrationEvents.TrueForAll(x => x.TotalDays == totalDayCount).Should().BeTrue();
            multiDayIntegrationEvents.Select(x => x.HearingConfirmationForParticipant)
                .Should().BeEquivalentTo(hearingConfirmationDtos);
        }
    }
}