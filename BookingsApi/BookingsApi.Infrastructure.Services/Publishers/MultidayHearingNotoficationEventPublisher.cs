using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class MultidayHearingNotoficationEventPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public MultidayHearingNotoficationEventPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.MultiDayHearingIntegrationEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var @case = videoHearing.GetCases()[0];
            foreach (var participant in videoHearing.Participants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new MultiDayHearingIntegrationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case)));
            }
        }
    }
}
