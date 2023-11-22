using BookingsApi.Common.Services;
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
        private readonly IFeatureToggles _featureToggles;
        public SingledayHearingAsynchronousProcess(IEventPublisherFactory publisherFactory, IFeatureToggles featureToggles)
        {
            _publisherFactory = publisherFactory;
            _featureToggles = featureToggles;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            if (!_featureToggles.UsePostMay2023Template())
            {
                await _publisherFactory.Get(EventType.CreateAndNotifyUserEvent).PublishAsync(videoHearing);
                await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(videoHearing);
                await _publisherFactory.Get(EventType.HearingNotificationEvent).PublishAsync(videoHearing);

                return;
            }
            await _publisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.NewParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.ExistingParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
        }
    }
}
