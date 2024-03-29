﻿using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IClonedBookingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing, int totalDays, DateTime videoHearingUpdateDate, bool sendNotificationNewParticipant = false);
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

        public async Task Start(VideoHearing videoHearing, int totalDays, DateTime videoHearingUpdateDate, bool sendNotificationNewParticipant = false)
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

            if (sendNotificationNewParticipant)
            {
                await _publisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).PublishAsync(videoHearing);
            }
            
            var publisherForNewParticipant = (MultidayHearingConfirmationforNewParticipantsPublisher)_publisherFactory.Get(EventType.NewParticipantMultidayHearingConfirmationEvent);
            publisherForNewParticipant.TotalDays = totalDays;
            publisherForNewParticipant.VideoHearingUpdateDate = videoHearingUpdateDate;
            await publisherForNewParticipant.PublishAsync(videoHearing);

            var publisherForExistingParticipant = (MultidayHearingConfirmationforExistingParticipantsPublisher)_publisherFactory.Get(EventType.ExistingParticipantMultidayHearingConfirmationEvent);
            publisherForExistingParticipant.TotalDays = totalDays;
            publisherForExistingParticipant.VideoHearingUpdateDate = videoHearingUpdateDate;
            await publisherForExistingParticipant.PublishAsync(videoHearing);
        }
    }
}
