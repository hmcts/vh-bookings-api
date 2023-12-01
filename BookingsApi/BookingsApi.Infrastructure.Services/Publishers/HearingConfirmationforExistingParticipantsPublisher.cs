using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;

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
            var existingParticipants = PublisherHelper.GetExistingParticipantsSinceLastUpdate(videoHearing);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in existingParticipants)
            {
                await _eventPublisher.PublishAsync(new ExistingParticipantHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case)));
            }
        }
    }
}
