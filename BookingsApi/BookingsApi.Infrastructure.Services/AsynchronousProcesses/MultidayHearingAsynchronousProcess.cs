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
        public ClonedMultidaysAsynchronousProcess(IEventPublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory; 
        }

        public async Task Start(VideoHearing videoHearing, int totalDays, DateTime videoHearingUpdateDate, bool sendNotificationNewParticipant = false)
        {
            if(totalDays <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalDays));
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
