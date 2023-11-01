using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class ClonedMultidaysAsynchronousProcessTests
    {
        private readonly ClonedMultidaysAsynchronousProcess _clonedMultidaysAsynchronousProcess;
        private readonly IEventPublisher _eventPublisher;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;

        public ClonedMultidaysAsynchronousProcessTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);

            _clonedMultidaysAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(_eventPublisher);
        }
        
        [Test]
        public async Task Should_publish_messages_for_single_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.Participants[0].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[0].Person, 
                hearing.Participants[0].Person.CreatedDate.AddDays(-10), null);
            hearing.Participants[1].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[1].Person,
                hearing.Participants[1].Person.CreatedDate.AddDays(-10), null);

            var createConfereceMessageCount = 0;
            var newParticipantWelcomeMessageCount = 0;
            var multidayHearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count - 2;
            var mulitdayHearingConfirmationForExistingParticipantsMessageCount = 2;
            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + multidayHearingConfirmationForNewParticipantsMessageCount
                + mulitdayHearingConfirmationForExistingParticipantsMessageCount;

            await _clonedMultidaysAsynchronousProcess.Start(hearing, 2);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue();
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent).
                Should().Be(multidayHearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent).
                Should().Be(mulitdayHearingConfirmationForExistingParticipantsMessageCount);
        }
    }
}