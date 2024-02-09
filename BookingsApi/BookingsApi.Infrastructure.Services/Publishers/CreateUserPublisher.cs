using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class CreateUserPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        
        public CreateUserPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.CreateUserEvent;
        
        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var newParticipants = PublisherHelper.GetNewParticipantsSinceLastUpdate(videoHearing).Where(x => x is not Judge);
            
            foreach (var participant in newParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new CreateUserIntegrationEvent(EventDtoMappers.MapToParticipantUserDto(
                    videoHearing.Id, participantDto)));
            }
        }
    }
}
