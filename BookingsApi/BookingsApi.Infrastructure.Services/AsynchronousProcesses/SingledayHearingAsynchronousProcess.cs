using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IBookingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }

    public class SingledayHearingAsynchronousProcess : IBookingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;
        public SingledayHearingAsynchronousProcess(IEventPublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            await _publisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.NewParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.ExistingParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.HearingNotificationForNewJudicialOfficersEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.HearingNotificationForJudiciaryParticipantEvent).PublishAsync(videoHearing);
        }
    }
}
