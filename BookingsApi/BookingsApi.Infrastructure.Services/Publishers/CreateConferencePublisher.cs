using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class CreateConferencePublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public CreateConferencePublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.CreateConferenceEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(videoHearing, videoHearing.Participants));
        }
    }
}
