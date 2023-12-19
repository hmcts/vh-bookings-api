using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class HearingConfirmationforNewParticipantsPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public HearingConfirmationforNewParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.NewParticipantHearingConfirmationEvent;
        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var newParticipants = PublisherHelper.GetNewParticipantsSinceLastUpdate(videoHearing);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new NewParticipantHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case)));
            }
        }
    }
}
