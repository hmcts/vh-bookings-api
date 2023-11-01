using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Services;

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

        public BookingServiceTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            _eventPublisher = new EventPublisher(_serviceBusQueueClient);
            _queryHandlerMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            _bookingAsynchronousProcess = new SingledayHearingAsynchronousProcess(_eventPublisher);
            _clonedBookingAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(_eventPublisher);
            _firstdayOfMultidayBookingAsynchronousProcess = new FirstdayOfMultidayHearingAsynchronousProcess(_eventPublisher);

            _bookingService = new BookingService(_eventPublisher, _commandHandlerMock.Object, _queryHandlerMock.Object,
                _bookingAsynchronousProcess, _firstdayOfMultidayBookingAsynchronousProcess, _clonedBookingAsynchronousProcess);
        }

        [Test]
        public async Task Should_publish_messages_for_single_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var createConfereceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count;
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count;

            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount;
            await _bookingService.PublishNewHearing(hearing, false);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue();
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(0);
        }

        [Test]
        public async Task Should_publish_messages_for_first_day_of_multi_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var createConfereceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count;
            var hearingConfirmationForNewParticipantsMessageCount = 0;

            var totalMessages = newParticipantWelcomeMessageCount + createConfereceMessageCount + hearingConfirmationForNewParticipantsMessageCount;
            await _bookingService.PublishNewHearing(hearing, true);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue();
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConfereceMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(0);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(0);
        }

        [Test]
        public async Task Should_publish_messages_for_multi_day_bookings_by_clone()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var newParticipantWelcomeMessageCount = 0;
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count;

            var totalMessages = newParticipantWelcomeMessageCount + hearingConfirmationForNewParticipantsMessageCount;
            await _bookingService.PublishMultiDayHearing(hearing, 2);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue();
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent).Should().Be(0);
        }
    }
}