using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IParticipantAddedToHearingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }

    public class ParticipantAddedToHearingAsynchronousProcess : IParticipantAddedToHearingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;
        private readonly IFeatureToggles _featureToggles;

        public ParticipantAddedToHearingAsynchronousProcess(IEventPublisherFactory publisherFactory, IFeatureToggles featureToggles)
        {
            _publisherFactory = publisherFactory;
            _featureToggles = featureToggles;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            if (!_featureToggles.UsePostMay2023Template())
            {
                await _publisherFactory.Get(EventType.CreateAndNotifyUserEvent).PublishAsync(videoHearing);
                await _publisherFactory.Get(EventType.HearingNotificationEvent).PublishAsync(videoHearing);
                return;
            }

            await _publisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.ParticipantAddedEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.NewParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.ExistingParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
        }
    }
}
