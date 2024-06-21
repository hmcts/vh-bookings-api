using BookingsApi.Common;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class ClonedMultidaysAsynchronousProcessTests
    {
        private readonly ClonedMultidaysAsynchronousProcess _clonedMultidaysAsynchronousProcess;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;
        public ClonedMultidaysAsynchronousProcessTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            IEventPublisher eventPublisher = new EventPublisher(_serviceBusQueueClient);
            IEventPublisherFactory eventPublisherFactory = EventPublisherFactoryInstance.Get(eventPublisher);
            _clonedMultidaysAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(eventPublisherFactory);
        }
        
        [Test]
        public async Task Should_publish_messages()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            // treat the first 2 participants an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(treatPersonAsNew: false).Build());
            hearing.Participants[1].ChangePerson(new PersonBuilder(treatPersonAsNew: false).Build());
            
            var judgeAsExistingParticipant = 1;
            var multidayHearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count(x => x is not Judge) - 2;
            var mulitdayHearingConfirmationForExistingParticipantsMessageCount = 2;
            var totalMessages = multidayHearingConfirmationForNewParticipantsMessageCount
                + mulitdayHearingConfirmationForExistingParticipantsMessageCount + judgeAsExistingParticipant;
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            
            await _clonedMultidaysAsynchronousProcess.Start(hearing, 2, videoHearingUpdateDate);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent).
                Should().Be(multidayHearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent).
                Should().Be(mulitdayHearingConfirmationForExistingParticipantsMessageCount + judgeAsExistingParticipant);
        }
    }
}