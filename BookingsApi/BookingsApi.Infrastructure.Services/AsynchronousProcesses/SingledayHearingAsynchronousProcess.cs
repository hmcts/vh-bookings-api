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
                await _publisherFactory.Get(EventType.HearingNotificationIntegrationEvent).PublishAsync(videoHearing);

                return;
            }
            await _publisherFactory.Get(EventType.WelcomeMessageForNewParticipantEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.HearingConfirmationForNewParticipantEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.HearingConfirmationForExistingParticipantEvent).PublishAsync(videoHearing);
        }
    }
}
