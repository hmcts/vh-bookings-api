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

        public EventType EventType => EventType.HearingConfirmationForExistingParticipantEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var existingParticipants = videoHearing.Participants.Where(x => x.DoesPersonAlreadyExist());
            var isUpdatedHearing = existingParticipants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            if (isUpdatedHearing)
            {
                existingParticipants = existingParticipants.Where(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds());
            }

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in existingParticipants)
            {
                await _eventPublisher.PublishAsync(new ExistingParticipantHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case)));
            }
        }
    }
}
