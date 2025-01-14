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
            
            var multiDayHearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count - 2;
            var multiDayHearingConfirmationForExistingParticipantsMessageCount = 2 + hearing.JudiciaryParticipants.Count;
            var totalMessages = multiDayHearingConfirmationForNewParticipantsMessageCount + multiDayHearingConfirmationForExistingParticipantsMessageCount;
            var videoHearingUpdateDate = hearing.UpdatedDate.Value.TrimSeconds();
            
            await _clonedMultidaysAsynchronousProcess.Start(hearing, 2, videoHearingUpdateDate);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent).
                Should().Be(multiDayHearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent).
                Should().Be(multiDayHearingConfirmationForExistingParticipantsMessageCount);
        }
    }
}