using BookingsApi.Common.Services;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Testing.Common.Stubs;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class FirstdayOfMultidayHearingAsynchronousProcessTest
    {
        private readonly FirstdayOfMultidayHearingAsynchronousProcess _firstdayOfMultidayHearingAsynchronousProcess;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;

        public FirstdayOfMultidayHearingAsynchronousProcessTest()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            IEventPublisher eventPublisher = new EventPublisher(_serviceBusQueueClient);
            IEventPublisherFactory eventPublisherFactory = EventPublisherFactoryInstance.Get(eventPublisher);
            IFeatureToggles featureToggles = new FeatureTogglesStub();
            _firstdayOfMultidayHearingAsynchronousProcess = new FirstdayOfMultidayHearingAsynchronousProcess(eventPublisherFactory, featureToggles);
        }

        [Test]
        public async Task Should_publish_messages_for_first_day_of_multiday_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            // treat the first 2 participants an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(true, treatPersonAsNew: false).Build());
            hearing.Participants[1].ChangePerson(new PersonBuilder(true, treatPersonAsNew: false).Build());

            var createConfereceMessageCount = 1;
            var newParticipantWelcomeMessageCount = 1;
            var hearingConfirmationForNewParticipantsMessageCount = 0;
            var hearingConfirmationForExistingParticipantsMessageCount = 0;
            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount
                + hearingConfirmationForExistingParticipantsMessageCount;

            await _firstdayOfMultidayHearingAsynchronousProcess.Start(hearing);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForExistingParticipantsMessageCount);
        }
    }
}