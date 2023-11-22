using BookingsApi.Common;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IFirstdayOfMultidayBookingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }

    public class FirstdayOfMultidayHearingAsynchronousProcess : IFirstdayOfMultidayBookingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;
        private readonly IFeatureToggles _featureToggles;
        public FirstdayOfMultidayHearingAsynchronousProcess(IEventPublisherFactory publisherFactory, IFeatureToggles featureToggles)
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

                return;
            }
            await _publisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(videoHearing);
        }
    }
}
