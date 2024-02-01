using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class MultidayHearingUpdateConfirmationForNewParticipantsPublisher : IPublishMultidayUpdateEvent
    {
        private readonly IEventPublisher _eventPublisher;

        public MultidayHearingUpdateConfirmationForNewParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.NewParticipantMultidayHearingUpdateConfirmationEvent;
   
        public async Task PublishAsync(VideoHearing videoHearing, IList<VideoHearing> multiDayHearings)
        {
            var newParticipants = PublisherHelper.GetNewParticipantsForMultiDaysSinceLastUpdate(videoHearing, multiDayHearings);

            var @case = videoHearing.GetCases()[0];
            foreach (var participant in newParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant.participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new NewParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case), participant.totalDays));
            }
        }
        
        /// <remarks>
        /// This overload is purely to comply with the base interface. Do not use
        /// </remarks>
        /// <deprecated>Use the overload with multiDayHearings instead</deprecated>
        public Task PublishAsync(VideoHearing videoHearing)
        {
            throw new InvalidOperationException();
        }
    }
}
