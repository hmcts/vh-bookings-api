using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;
using BookingsApi.Common;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class WelcomeEmailForNewParticipantsPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public WelcomeEmailForNewParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.NewParticipantWelcomeEmailEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var videoHearingUpdateDate = videoHearing.UpdatedDate.TrimSeconds();
            var newParticipants =
                PublisherHelper.GetNewParticipantsSinceLastUpdate(videoHearing, videoHearingUpdateDate);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant);
                
                await _eventPublisher.PublishAsync(new NewParticipantWelcomeEmailEvent(EventDtoMappers.MapToWelcomeEmailDto(
                    videoHearing.Id, participantDto, @case)));
            }
        }
    }
}
