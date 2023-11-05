using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using System.Collections.Generic;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class FirstdayOfMultidayHearingAsynchronousProcessTest
    {
        private readonly FirstdayOfMultidayHearingAsynchronousProcess _firstdayOfMultidayHearingAsynchronousProcess;
        private readonly IEventPublisher _eventPublisher;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;
        private readonly IEventPublisherFactory _eventPublisherFactory;

        public FirstdayOfMultidayHearingAsynchronousProcessTest()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);
            _eventPublisherFactory = new EventPublisherFactory(new List<IPublishEvent> {
                new WelcomeEmailForNewParticipantsPublisher(_eventPublisher),
                new HearingConfirmationforNewParticipantsPublisher(_eventPublisher),
                new CreateConferencePublisher(_eventPublisher),
                new HearingConfirmationforNewParticipantsPublisher(_eventPublisher),
                new HearingConfirmationforExistingParticipantsPublisher(_eventPublisher),
                new MultidayHearingConfirmationforNewParticipantsPublisher(_eventPublisher),
                new MultidayHearingConfirmationforExistingParticipantsPublisher(_eventPublisher)});

            _firstdayOfMultidayHearingAsynchronousProcess = new FirstdayOfMultidayHearingAsynchronousProcess(_eventPublisherFactory);
        }

        [Test]
        public async Task Should_publish_messages_for_first_day_of_multiday_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.Participants[0].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[0].Person,
                hearing.Participants[0].Person.CreatedDate.AddDays(-10), null);
            hearing.Participants[1].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[1].Person,
                hearing.Participants[1].Person.CreatedDate.AddDays(-10), null);

            var createConfereceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count - 2;
            var hearingConfirmationForNewParticipantsMessageCount = 0;
            var hearingConfirmationForExistingParticipantsMessageCount = 0;
            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount
                + hearingConfirmationForExistingParticipantsMessageCount;

            await _firstdayOfMultidayHearingAsynchronousProcess.Start(hearing);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue();
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForExistingParticipantsMessageCount);
        }
    }
}