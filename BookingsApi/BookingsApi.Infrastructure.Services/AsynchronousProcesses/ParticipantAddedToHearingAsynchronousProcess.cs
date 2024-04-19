using System;
using System.Collections.Generic;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IParticipantAddedToHearingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing, IList<Guid> participantsToBeNotifiedIds);
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

        public async Task Start(VideoHearing videoHearing, IList<Guid> participantsToBeNotifiedIds)
        {
            if (!_featureToggles.UsePostMay2023Template())
            {
                var createAndNotifyPublisher = (CreateAndNotifyUserPublisher)_publisherFactory.Get(EventType.CreateAndNotifyUserEvent);
                createAndNotifyPublisher.ParticipantsToBeNotifiedIds = participantsToBeNotifiedIds;
                await createAndNotifyPublisher.PublishAsync(videoHearing);

                var hearingNotifyPublisher = (HearingNotificationEventPublisher)_publisherFactory.Get(EventType.HearingNotificationEvent);
                hearingNotifyPublisher.ParticipantsToBeNotifiedIds = participantsToBeNotifiedIds;
                await hearingNotifyPublisher.PublishAsync(videoHearing);

                return;
            }

            var publisherForWelcomeEmail = (WelcomeEmailForNewParticipantsPublisher) _publisherFactory
                .Get(EventType.NewParticipantWelcomeEmailEvent);
            publisherForWelcomeEmail.ParticipantsToBeNotifiedIds = participantsToBeNotifiedIds;
            await publisherForWelcomeEmail.PublishAsync(videoHearing);
            
            var publisherForNewParticipant = (HearingConfirmationforNewParticipantsPublisher)_publisherFactory.Get(EventType.NewParticipantHearingConfirmationEvent);
            publisherForNewParticipant.ParticipantsToBeNotifiedIds = participantsToBeNotifiedIds;
            await publisherForNewParticipant.PublishAsync(videoHearing);

            var publisherForExistingParticipant = (HearingConfirmationforExistingParticipantsPublisher)_publisherFactory.Get(EventType.ExistingParticipantHearingConfirmationEvent);
            publisherForExistingParticipant.ParticipantsToBeNotifiedIds = participantsToBeNotifiedIds;
            await publisherForExistingParticipant.PublishAsync(videoHearing);
        }
    }
}
