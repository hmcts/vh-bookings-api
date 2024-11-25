using System;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    [Obsolete("JudicialOfficeHolder are no longer created as Participants")]
    public class HearingNotificationEventForNewJudicialOfficersPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public HearingNotificationEventForNewJudicialOfficersPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.HearingNotificationForNewJudicialOfficersEvent;

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var @case = videoHearing.GetCases()[0];
            
            // we need to send a hearing confirmation for new Panel Member created for V1 and send new templates. A create email for those users
            // has been sent previously with login details and needs a second email for hearing confirmation
            var newJudicialOfficers = PublisherHelper.GetNewParticipantsSinceLastUpdate(videoHearing, videoHearing.UpdatedDate.TrimSeconds())
                .Where(x => x is JudicialOfficeHolder);
            
            foreach (var participant in newJudicialOfficers)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant);
                await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case)));
            }
        }
    }
}
