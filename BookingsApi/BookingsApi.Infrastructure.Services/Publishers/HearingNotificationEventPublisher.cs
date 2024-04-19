using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class HearingNotificationEventPublisher : IPublishEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public HearingNotificationEventPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.HearingNotificationEvent;
        public IList<Guid> ParticipantsToBeNotifiedIds { get; set; }

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            if (ParticipantsToBeNotifiedIds == null)
                ParticipantsToBeNotifiedIds = videoHearing.Participants.Select(p => p.Id).ToList();
            var addedParticipants = PublisherHelper.GetAddedParticipantsSinceLastUpdate(videoHearing, ParticipantsToBeNotifiedIds);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in addedParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case)));
            }
        }
    }
}
