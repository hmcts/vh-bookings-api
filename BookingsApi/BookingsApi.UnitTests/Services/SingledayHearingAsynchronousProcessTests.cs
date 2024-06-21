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
    public class SingledayHearingAsynchronousProcessTests
    {
        private readonly SingledayHearingAsynchronousProcess _singledayHearingAsynchronousProcess;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;

        public SingledayHearingAsynchronousProcessTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            IEventPublisher eventPublisher = new EventPublisher(_serviceBusQueueClient);
            IEventPublisherFactory eventPublisherFactory = EventPublisherFactoryInstance.Get(eventPublisher);

            _singledayHearingAsynchronousProcess = new SingledayHearingAsynchronousProcess(eventPublisherFactory);
        }

        [Test]
        public async Task Should_publish_messages_for_single_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            // treat the first participant an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(true, treatPersonAsNew: false).Build());

            var createConfereceMessageCount = 1;
            var judgeAsExistingParticipant = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count(x => x is not JudicialOfficeHolder && x is not Judge) - 1;
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count - 2;
            var hearingConfirmationForExistingParticipantsMessageCount = 1 + judgeAsExistingParticipant;
            var updateDate = hearing.UpdatedDate.TrimSeconds();
            var judicialOfficerAsNewParticipant = PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing, updateDate).Count(x => x is JudicialOfficeHolder);

            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount
                                + hearingConfirmationForExistingParticipantsMessageCount + judicialOfficerAsNewParticipant;

            await _singledayHearingAsynchronousProcess.Start(hearing);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForExistingParticipantsMessageCount);
        }
        
        [Test]
        public async Task Should_publish_messages_for_single_day_booking_with_panel_member_and_ejud_new_template_on()
        {
            var hearing = new VideoHearingBuilder(addJudge:false).WithCase().WithJudiciaryPanelMember().WithJudiciaryJudge().Build();
            // treat the first participant an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(true, treatPersonAsNew: false).Build());
            
            
            var createConferenceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count(x => x is not JudicialOfficeHolder && x is not Judge) - 1;
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count - 1;
            var hearingConfirmationForExistingParticipantsMessageCount = 1;
            var updateDate = hearing.UpdatedDate.TrimSeconds();
            var judicialOfficerAsNewParticipant = PublisherHelper.GetNewParticipantsSinceLastUpdate(hearing, updateDate).Count(x => x is JudicialOfficeHolder);
            var totalMessages = newParticipantWelcomeMessageCount + createConferenceMessageCount + hearingConfirmationForNewParticipantsMessageCount
                                + hearingConfirmationForExistingParticipantsMessageCount + hearing.JudiciaryParticipants.Count + judicialOfficerAsNewParticipant;

            await _singledayHearingAsynchronousProcess.Start(hearing);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConferenceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForExistingParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingNotificationIntegrationEvent).Should().Be(hearing.JudiciaryParticipants.Count + judicialOfficerAsNewParticipant);
        }
    }
}