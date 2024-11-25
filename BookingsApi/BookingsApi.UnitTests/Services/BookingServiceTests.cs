using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Services;
using BookingsApi.Common;

namespace BookingsApi.UnitTests.Services
{
    [TestFixture]
    public class BookingServiceTests
    {
        private readonly BookingService _bookingService;
        private readonly ServiceBusQueueClientFake _serviceBusQueueClient;


        public BookingServiceTests()
        {
            _serviceBusQueueClient = new ServiceBusQueueClientFake();
            IEventPublisher eventPublisher = new EventPublisher(_serviceBusQueueClient);
            var queryHandlerMock = new Mock<IQueryHandler>();
            var commandHandlerMock = new Mock<ICommandHandler>();
            IEventPublisherFactory eventPublisherFactory = EventPublisherFactoryInstance.Get(eventPublisher);

            IBookingAsynchronousProcess bookingAsynchronousProcess = new SingledayHearingAsynchronousProcess(eventPublisherFactory);
            IClonedBookingAsynchronousProcess clonedBookingAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(eventPublisherFactory);
            IFirstdayOfMultidayBookingAsynchronousProcess firstdayOfMultidayBookingAsynchronousProcess = new FirstdayOfMultidayHearingAsynchronousProcess(eventPublisherFactory);
            ICreateConferenceAsynchronousProcess createConferenceAsynchronousProcess = new CreateConferenceAsynchronousProcess(eventPublisherFactory);
            _bookingService = new BookingService(eventPublisher, commandHandlerMock.Object, queryHandlerMock.Object,
                bookingAsynchronousProcess, firstdayOfMultidayBookingAsynchronousProcess, clonedBookingAsynchronousProcess, 
                createConferenceAsynchronousProcess);
        }

        [Test]
        public async Task Should_publish_messages_for_single_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var createConferenceMessageCount = 1; // hearing has a host so it can be marked as ready for video
            var newParticipantWelcomeMessageCount = hearing.Participants.Count; //  all participants treated as new to the hearing in this scenario
            var newParticipantConfirmationMessageCount = hearing.Participants.Count; // all participants treated as new to the hearing in this scenario
            var judiciaryHearingNotificationCount = hearing.JudiciaryParticipants.Count; // all judiciary participants treated as new to this hearing get a hearing confirmation
            
            
            var totalMessages = newParticipantWelcomeMessageCount + createConferenceMessageCount + newParticipantConfirmationMessageCount + judiciaryHearingNotificationCount;
            await _bookingService.PublishNewHearing(hearing, false);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(newParticipantConfirmationMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConferenceMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingNotificationIntegrationEvent).Should().Be(judiciaryHearingNotificationCount);
        }
        
        [Test]
        public async Task Should_publish_messages_for_first_day_of_multi_day_booking()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var createConferenceMessageCount = 1;
            var newParticipantWelcomeMessageCount = hearing.Participants.Count;
            var hearingConfirmationForNewParticipantsMessageCount = 0;

            var totalMessages = newParticipantWelcomeMessageCount + createConferenceMessageCount + hearingConfirmationForNewParticipantsMessageCount;
            await _bookingService.PublishNewHearing(hearing, true);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantWelcomeEmailEvent).Should().Be(newParticipantWelcomeMessageCount);
            messages.Count(x => x.IntegrationEvent is HearingIsReadyForVideoIntegrationEvent).Should().Be(createConferenceMessageCount);
            messages.Count(x => x.IntegrationEvent is NewParticipantHearingConfirmationEvent).Should().Be(0);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantHearingConfirmationEvent).Should().Be(0);
        }

        [Test]
        public async Task Should_publish_messages_for_multi_day_bookings_by_clone()
        {
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.IsFirstDayOfMultiDayHearing = true;
            var videoHearingUpdateDate = hearing.UpdatedDate.TrimSeconds();
            var hearingConfirmationForNewParticipantsMessageCount = hearing.Participants.Count;  //  all participants treated as new to the hearing in this scenario
            var judiciaryMultiDayHearingNotificationCount = hearing.JudiciaryParticipants.Count; // all judiciary participants treated as new to this hearing get a hearing confirmation
            var totalMessages = hearingConfirmationForNewParticipantsMessageCount + judiciaryMultiDayHearingNotificationCount;

            await _bookingService.PublishMultiDayHearing(hearing, 2, videoHearingUpdateDate);

            var messages = _serviceBusQueueClient.ReadAllMessagesFromQueue(hearing.Id);
            messages.Length.Should().Be(totalMessages);

            messages.Count(x => x.IntegrationEvent is NewParticipantMultidayHearingConfirmationEvent).Should().Be(hearingConfirmationForNewParticipantsMessageCount);
            messages.Count(x => x.IntegrationEvent is ExistingParticipantMultidayHearingConfirmationEvent).Should().Be(judiciaryMultiDayHearingNotificationCount);
        }
    }
}