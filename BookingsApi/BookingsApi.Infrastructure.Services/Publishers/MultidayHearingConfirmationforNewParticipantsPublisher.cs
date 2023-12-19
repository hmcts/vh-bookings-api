using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class MultidayHearingConfirmationforNewParticipantsPublisher: IPublishMultidayEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public MultidayHearingConfirmationforNewParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.NewParticipantMultidayHearingConfirmationEvent;
        public int TotalDays { get; set; }

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var newParticipants = PublisherHelper.GetNewParticipantsSinceLastUpdate(videoHearing);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new NewParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case), TotalDays));
            }
        }
    }
}
