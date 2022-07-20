﻿using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingIsReadyForVideoIntegrationEvent : IIntegrationEvent
    {
        public HearingIsReadyForVideoIntegrationEvent(Hearing hearing, IEnumerable<Participant> newParticipants)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            
            var hearingParticipants = hearing.GetParticipants();
            Participants = hearingParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
            foreach(var newParticipant in newParticipants)
            {
                var participant = Participants.SingleOrDefault(x => x.ParticipantId == newParticipant.Id);
                if(participant != null)
                {
                    participant.SendHearingNotificationIfNew = true;
                }
            }

            Participants.SingleOrDefault(x => x.UserRole == "Judge")?.SetOtherFieldsForNonEJudJudgeUser(hearing.OtherInformation);
            var hearingEndpoints = hearing.GetEndpoints();
            Endpoints = hearingEndpoints.Select(EndpointDtoMapper.MapToDto).ToList();
        }

        public HearingDto Hearing { get; }
        public IList<ParticipantDto> Participants { get; }
        public IList<EndpointDto> Endpoints { get; set; }
    }
}

    