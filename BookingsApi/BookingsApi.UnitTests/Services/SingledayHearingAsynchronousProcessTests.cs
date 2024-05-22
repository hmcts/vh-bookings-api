using BookingsApi.Common;
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
    public class SingledayHearingAsynchronousProcessTests
    {
        private readonly SingledayHearingAsynchronousProcess _singledayHearingAsynchronousProcess;
        private readonly IEventPublisher _eventPublisher;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;
        private readonly IEventPublisherFactory _eventPublisherFactory;
        private readonly IFeatureToggles _featureToggles;
        public SingledayHearingAsynchronousProcessTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);
            _eventPublisherFactory = EventPublisherFactoryInstance.Get(_eventPublisher);
            _featureToggles = new FeatureTogglesStub();

            _singledayHearingAsynchronousProcess = new SingledayHearingAsynchronousProcess(_eventPublisherFactory, _featureToggles);
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
        public async Task Should_publish_messages_for_single_day_booking_with_panel_member_and_ejud_new_template_off()
        {
            var hearing = new VideoHearingBuilder(addJudge:false).WithCase().WithJudiciaryPanelMember().WithJudiciaryJudge().Build();
            // treat the first participant an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(true, treatPersonAsNew: false).Build());
           
            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;

            
            var createConferenceMessageCount = 1;
            var judgeAsExistingParticipant = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count(x => x is not JudicialOfficeHolder && x is not Judge);
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count;
            var hearingConfirmationForExistingParticipantsMessageCount = 1 + judgeAsExistingParticipant;
            
            var totalMessages = newParticipantWelcomeMessageCount + createConferenceMessageCount + hearingConfirmationForNewParticipantsMessageCount
                                + hearingConfirmationForExistingParticipantsMessageCount;
            
            await _singledayHearingAsynchronousProcess.Start(hearing);
            
            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is CreateAndNotifyUserIntegrationEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConferenceMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingNotificationIntegrationEvent).Should().Be(hearing.JudiciaryParticipants.Count + hearing.Participants.Count);
        }
        
        [Test]
        public async Task Should_publish_messages_for_single_day_booking_with_panel_member_and_ejud_new_template_on()
        {
            var hearing = new VideoHearingBuilder(addJudge:false).WithCase().WithJudiciaryPanelMember().WithJudiciaryJudge().Build();
            // treat the first participant an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(true, treatPersonAsNew: false).Build());
            
            ((FeatureTogglesStub)_featureToggles).NewTemplates = true;
            
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

        [Test]
        public async Task Should_publish_messages_for_user_setup_before_booking_but_no_Account_created()
        {
            var hearing = new VideoHearingBuilder(addJudge: false).WithCase().WithJudiciaryPanelMember().WithJudiciaryJudge().Build();
            // treat the first participant an existing person
            hearing.Participants[0].ChangePerson(new PersonBuilder(true, treatPersonAsNew: false).Build());

            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;

            var createConferenceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count(x => x is not JudicialOfficeHolder && x is not Judge);
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count;
            var totalMessages = newParticipantWelcomeMessageCount + createConferenceMessageCount + hearingConfirmationForNewParticipantsMessageCount
                                 + hearing.JudiciaryParticipants.Count;
            
            await _singledayHearingAsynchronousProcess.Start(hearing);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is CreateAndNotifyUserIntegrationEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConferenceMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingNotificationIntegrationEvent).Should().Be(hearing.JudiciaryParticipants.Count + hearing.Participants.Count);
        }

    }
}