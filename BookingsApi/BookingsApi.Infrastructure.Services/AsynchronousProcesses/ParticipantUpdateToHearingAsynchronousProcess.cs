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

    public class ParticipantUpdateToHearingAsynchronousProcess : IParticipantUpdateToHearingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;
        private readonly IFeatureToggles _featureToggles;

        public ParticipantUpdateToHearingAsynchronousProcess(IEventPublisherFactory publisherFactory, IFeatureToggles featureToggles)
        {
            _publisherFactory = publisherFactory;
            _featureToggles = featureToggles;
        }

        public async Task Start(VideoHearing hearing)
        {
            var participants = hearing.GetJudiciaryParticipants()
                .Where(x=> x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge)
                .ToList();
        

            switch (hearing.Status)
            {
                case BookingStatus.Created or BookingStatus.ConfirmedWithoutJudge:
                    await _publisherFactory.Get(EventType.ParticipantAddedEvent).PublishAsync(hearing);
                    await _publisherFactory.Get(EventType.HearingNotificationForJudiciaryParticipantEvent).PublishAsync(hearing);
                    break;
                case BookingStatus.Booked or BookingStatus.BookedWithoutJudge when participants.Exists(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge):
                    await _publisherFactory.Get(EventType.CreateConferenceEvent).PublishAsync(hearing);
                    break;
            }
            
        }
    }
}
