using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class ParticipantAddedPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public ParticipantAddedPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.ParticipantAddedEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(videoHearing, videoHearing.Participants));
        }
    }
}
