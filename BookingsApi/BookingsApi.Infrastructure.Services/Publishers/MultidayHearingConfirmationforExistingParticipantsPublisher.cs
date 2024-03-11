﻿using System;
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
        public DateTime VideoHearingUpdateDate { get; set; }

        public async Task PublishAsync(VideoHearing videoHearing)
        {
            var existingParticipants = PublisherHelper.GetExistingParticipantsSinceLastUpdate(videoHearing, VideoHearingUpdateDate);

            var @case = videoHearing.GetCases()[0];
            
            foreach (var participant in existingParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, videoHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new ExistingParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case), TotalDays));
            }
            
            var existingJudiciaryParticipants = PublisherHelper.GetAddedJudiciaryParticipantsSinceLastUpdate(videoHearing, VideoHearingUpdateDate);
            
            foreach (var participant in existingJudiciaryParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant);
                
                await _eventPublisher.PublishAsync(new ExistingParticipantMultidayHearingConfirmationEvent(EventDtoMappers.MapToHearingConfirmationDto(
                    videoHearing.Id, videoHearing.ScheduledDateTime, participantDto, @case), TotalDays));
            }
        }
    }
}
