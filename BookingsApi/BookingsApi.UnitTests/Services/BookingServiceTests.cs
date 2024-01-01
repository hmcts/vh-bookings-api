using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Services;
using System.Collections.Generic;
using Testing.Common.Stubs;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class BookingServiceTests
    {
        private readonly BookingService _bookingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly Mock<IQueryHandler> _queryHandlerMock;
        private readonly Mock<ICommandHandler> _commandHandlerMock;
        private readonly IBookingAsynchronousProcess _bookingAsynchronousProcess;
        private readonly IClonedBookingAsynchronousProcess _clonedBookingAsynchronousProcess;
        private readonly IFirstdayOfMultidayBookingAsynchronousProcess _firstdayOfMultidayBookingAsynchronousProcess;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;
        private readonly IEventPublisherFactory _eventPublisherFactory;
        private readonly IFeatureToggles _featureToggles;

        public BookingServiceTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);
            _queryHandlerMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            _eventPublisherFactory = EventPublisherFactoryInstance.Get(_eventPublisher);
            _featureToggles = new FeatureTogglesStub();

            _bookingAsynchronousProcess = new SingledayHearingAsynchronousProcess(_eventPublisherFactory, _featureToggles);
            _clonedBookingAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(_eventPublisherFactory, _featureToggles);
            _firstdayOfMultidayBookingAsynchronousProcess = new FirstdayOfMultidayHearingAsynchronousProcess(_eventPublisherFactory, _featureToggles);

            _bookingService = new BookingService(_eventPublisher, _commandHandlerMock.Object, _queryHandlerMock.Object,
                _bookingAsynchronousProcess, _firstdayOfMultidayBookingAsynchronousProcess, _clonedBookingAsynchronousProcess);
        }

        [Test]
        public async Task Should_publish_messages_for_single_day_booking()
        {
            ((FeatureTogglesStub)_featureToggles).NewTemplates = true;
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var createConfereceMessageCount = 1;
            var judgeAsExistingParticipant = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count(x => x is not Judge && x is not JudicialOfficeHolder);
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count(x => x is not Judge);

            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount + judgeAsExistingParticipant;
            await _bookingService.PublishNewHearing(hearing, false);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(judgeAsExistingParticipant);
        }

        [Test]
        public async Task Should_publish_messages_for_single_day_booking_for_old_templates()
        {
            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var createConfereceMessageCount = 1;
            var newParticipantMessageCount = hearing.Participants.Count(x => x is not Judge);
            var hearingNotificationMessageCount = hearing.Participants.Count();

            var totalMessages = newParticipantMessageCount + createConfereceMessageCount + hearingNotificationMessageCount;
            await _bookingService.PublishNewHearing(hearing, false);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is CreateAndNotifyUserIntegrationEvent).Should().Be(newParticipantMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingNotificationIntegrationEvent).Should().Be(hearingNotificationMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
        }

        [Test]
        public async Task Should_publish_messages_for_first_day_of_multi_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var createConfereceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count(x => x is not Judge && x is not JudicialOfficeHolder);
            var hearingConfirmationForNewParticipantsMessageCount = 0;

            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount;
            await _bookingService.PublishNewHearing(hearing, true);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(0);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(0);
        }

        [Test]
        public async Task Should_publish_messages_for_first_day_of_multi_day_booking_old_templates()
        {
            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var createConfereceMessageCount = 1;
            var newParticipantMessageCount = hearing.Participants.Count(x => x is not Judge);
            var hearingNotificationMessageCount = 0;

            var totalMessages = newParticipantMessageCount + createConfereceMessageCount + hearingNotificationMessageCount;
            await _bookingService.PublishNewHearing(hearing, true);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is CreateAndNotifyUserIntegrationEvent).Should().Be(newParticipantMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
        }

        [Test]
        public async Task Should_publish_messages_for_multi_day_bookings_by_clone()
        {
            ((FeatureTogglesStub)_featureToggles).NewTemplates = true;
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var existingParticipantMessageCount = 1;
            var judge = hearing.Participants.Single(x => x is Judge);
            judge.Person.GetType().GetProperty("CreatedDate").SetValue(judge.Person, judge.Person.CreatedDate.AddDays(-10), null);
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count(x => x is not Judge);
            var totalMessages = existingParticipantMessageCount + hearingConfirmationForNewParticipantsMessageCount;

            await _bookingService.PublishMultiDayHearing(hearing, 2);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent).Should().Be(existingParticipantMessageCount);
        }

        [Test]
        public async Task Should_publish_messages_for_multi_day_bookings_by_clone_old_templates()
        {
            ((FeatureTogglesStub)_featureToggles).NewTemplates = false;
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var hearingParticipantsMessageCount = hearing.Participants.Count();

            await _bookingService.PublishMultiDayHearing(hearing, 2);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(hearingParticipantsMessageCount);

            messages.Count(x => x.IntegrationEvent is MultiDayHearingIntegrationEvent).Should().Be(hearingParticipantsMessageCount);
        }
    }
}