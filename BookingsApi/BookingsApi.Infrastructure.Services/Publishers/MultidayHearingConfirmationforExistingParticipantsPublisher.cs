using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class MultidayHearingConfirmationforExistingParticipantsPublisher: IPublishMultidayEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public MultidayHearingConfirmationforExistingParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.MultidayHearingConfirmationforExistingParticipantEvent;
        public int TotalDays { get; set; }

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var existingParticipants = videoHearing.Participants.Where(x => x.DoesPersonAlreadyExist());
            var isUpdatedHearing = existingParticipants.Any(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds());
            if (isUpdatedHearing)
            {
                existingParticipants = existingParticipants.Where(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds());
            }

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in existingParticipants)
            {
                await _eventPublisher.PublishAsync(new ExistingParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case), TotalDays));
            }
        }
    }
}
