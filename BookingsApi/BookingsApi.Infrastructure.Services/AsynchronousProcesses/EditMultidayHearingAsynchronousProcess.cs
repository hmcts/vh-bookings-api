using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IEditMultidayHearingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing, int totalDays);
    }

    public class EditMultidayHearingAsynchronousProcess: IEditMultidayHearingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;
        private readonly IFeatureToggles _featureToggles;
        public EditMultidayHearingAsynchronousProcess(IEventPublisherFactory publisherFactory, IFeatureToggles featureToggles)
        {
            _publisherFactory = publisherFactory;
            _featureToggles = featureToggles;
        }

        public async Task Start(VideoHearing videoHearing, int totalDays)
        {
            if(totalDays <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalDays));
            }
            if (!_featureToggles.UsePostMay2023Template())
            {
                await _publisherFactory.Get(EventType.CreateAndNotifyUserEvent).PublishAsync(videoHearing);

                var publisherForMultiDayEvent = (IPublishMultidayEvent)_publisherFactory.Get(EventType.MultiDayHearingIntegrationEvent);
                publisherForMultiDayEvent.TotalDays = totalDays;
                await publisherForMultiDayEvent.PublishAsync(videoHearing);
                
                return;
            }
            
            await _publisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).PublishAsync(videoHearing);

            var publisherForNewParticipant = (IPublishMultidayEvent)_publisherFactory.Get(EventType.NewParticipantMultidayHearingConfirmationEvent);
            publisherForNewParticipant.TotalDays = totalDays;
            await publisherForNewParticipant.PublishAsync(videoHearing);

            var publisherForExistingParticipant = (IPublishMultidayEvent)_publisherFactory.Get(EventType.ExistingParticipantMultidayHearingConfirmationEvent);
            publisherForExistingParticipant.TotalDays = totalDays;
            await publisherForExistingParticipant.PublishAsync(videoHearing);
        }
    }
}
