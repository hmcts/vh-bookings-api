using System;
using System.Collections.Generic;
using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class HearingNotificationEventForJudiciaryParticipantPublisher : IPublishJudiciaryParticipantsEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public HearingNotificationEventForJudiciaryParticipantPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.HearingNotificationForJudiciaryParticipantEvent;

        public async Task PublishAsync(VideoHearing videoHearing, IList<JudiciaryParticipant> newJudiciaryParticipants)
        {
            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newJudiciaryParticipants)
            {
                await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case)));
            }
        }
        
        /// <remarks>
        /// This overload is purely to comply with the base interface. Do not use
        /// </remarks>
        /// <deprecated>Use the overload with judiciaryParticipants instead</deprecated>
        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var @case = videoHearing.GetCases()[0];
            foreach (var participant in videoHearing.JudiciaryParticipants)
            {
                await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participant, @case)));
            }
        }
    }
}
