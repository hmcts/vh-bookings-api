using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;
using BookingsApi.Common;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class HearingConfirmationforExistingParticipantsPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public HearingConfirmationforExistingParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.ExistingParticipantHearingConfirmationEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var updateDate = videoHearing.UpdatedDate?.TrimSeconds();
            var existingParticipants = PublisherHelper.GetExistingParticipantsSinceLastUpdate(videoHearing, updateDate.Value);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in existingParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant);
                
                await _eventPublisher.PublishAsync(new ExistingParticipantHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case)));
            }
        }
    }
}
