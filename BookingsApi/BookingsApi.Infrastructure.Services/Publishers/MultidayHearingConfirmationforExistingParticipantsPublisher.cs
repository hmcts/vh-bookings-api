using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using System.Threading.Tasks;
using BookingsApi.Common;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public class MultidayHearingConfirmationforExistingParticipantsPublisher: IPublishMultidayEvent
    {
        private readonly IEventPublisher _eventPublisher;
        public MultidayHearingConfirmationforExistingParticipantsPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public EventType EventType => EventType.ExistingParticipantMultidayHearingConfirmationEvent;
        public int TotalDays { get; set; }
        
        public IList<Guid> ParticipantsToBeNotifiedIds { get; set; }
        public IList<Guid> JudiciaryParticipantsToBeNotifiedIds { get; set; }  

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var existingParticipants = PublisherHelper.GetExistingParticipantsSinceLastUpdate(videoHearing, ParticipantsToBeNotifiedIds);

            var @case = videoHearing.GetCases()[0];
            
            foreach (var participant in existingParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new ExistingParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case), TotalDays));
            }
            
            var existingJudiciaryParticipants = PublisherHelper.GetAddedJudiciaryParticipantsSinceLastUpdate(videoHearing, JudiciaryParticipantsToBeNotifiedIds);
            
            foreach (var participant in existingJudiciaryParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant);
                
                await _eventPublisher.PublishAsync(new ExistingParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case), TotalDays));
            }
        }
    }
}
