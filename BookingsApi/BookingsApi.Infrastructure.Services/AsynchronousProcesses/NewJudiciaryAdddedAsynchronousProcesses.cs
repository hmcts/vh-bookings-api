using System.Linq;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Threading.Tasks;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IParticipantUpdateToHearingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }

    public class NewJudiciaryAdddedAsynchronousProcesses : IParticipantUpdateToHearingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;

        public NewJudiciaryAdddedAsynchronousProcesses(IEventPublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            var participants = videoHearing.GetJudiciaryParticipants().ToList();
        

            switch (videoHearing.Status)
            {
                case BookingStatus.Created or BookingStatus.ConfirmedWithoutJudge:
                    await _publisherFactory.Get(EventType.ParticipantAddedEvent).PublishAsync(videoHearing);
                    await _publisherFactory.Get(EventType.HearingNotificationForJudiciaryParticipantEvent).PublishAsync(videoHearing);
                    break;
                case BookingStatus.Booked or BookingStatus.BookedWithoutJudge when participants.Exists(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge):
                    await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(videoHearing);
                    break;
            }
            
        }
    }
}
