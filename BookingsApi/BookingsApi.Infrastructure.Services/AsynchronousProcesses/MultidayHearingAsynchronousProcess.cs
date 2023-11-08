using BookingsApi.Common;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.Publishers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IClonedBookingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing, int totalDays);
    }

    public class ClonedMultidaysAsynchronousProcess: IClonedBookingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;
        private readonly IFeatureToggles _featureToggles;
        public ClonedMultidaysAsynchronousProcess(IEventPublisherFactory publisherFactory, IFeatureToggles featureToggles)
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
                await _publisherFactory.Get(EventType.MultiDayHearingIntegrationEvent).PublishAsync(videoHearing);

                return;
            }

            var publisherForNewParticipant = (IPublishMultidayEvent)_publisherFactory.Get(EventType.MultidayHearingConfirmationforNewParticipantEvent);
            publisherForNewParticipant.TotalDays = totalDays;
            await publisherForNewParticipant.PublishAsync(videoHearing);

            var publisherForExistingParticipant = (IPublishMultidayEvent)_publisherFactory.Get(EventType.MultidayHearingConfirmationforExistingParticipantEvent);
            publisherForExistingParticipant.TotalDays = totalDays;
            await publisherForExistingParticipant.PublishAsync(videoHearing);
        }
    }
}
