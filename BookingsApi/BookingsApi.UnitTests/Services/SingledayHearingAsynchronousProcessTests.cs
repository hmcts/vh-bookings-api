using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using System.Collections.Generic;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class SingledayHearingAsynchronousProcessTests
    {
        private readonly SingledayHearingAsynchronousProcess _singledayHearingAsynchronousProcess;
        private readonly IEventPublisher _eventPublisher;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;
        private readonly IEventPublisherFactory _eventPublisherFactory;
        public SingledayHearingAsynchronousProcessTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);
            _eventPublisherFactory = EventPublisherFactoryInstance.Get(_eventPublisher);

            _singledayHearingAsynchronousProcess = new SingledayHearingAsynchronousProcess(_eventPublisherFactory);
        }
        
        [Test]
        public async Task Should_publish_messages_for_single_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.Participants[0].Person.GetType().GetProperty("CreatedDate").SetValue(hearing.Participants[0].Person, 
                hearing.Participants[0].Person.CreatedDate.AddDays(-10), null);

            var createConfereceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count(x => x is Individual) - 1;
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count - 2;
            var hearingConfirmationForExistingParticipantsMessageCount = 1;
            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount
                + hearingConfirmationForExistingParticipantsMessageCount;

            await _singledayHearingAsynchronousProcess.Start(hearing);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue();
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForExistingParticipantsMessageCount);
        }
    }
}