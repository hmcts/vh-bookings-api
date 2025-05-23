﻿using System;
using System.Collections.Generic;
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
            
            var hearingParticipants = hearing.GetParticipants().Where(ParticipantHasContactEmail());
            Participants = hearingParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
            foreach(var newParticipant in newParticipants.Where(ParticipantHasContactEmail()))
            {
                var participant = Participants.SingleOrDefault(x => x.ParticipantId == newParticipant.Id);
                if(participant != null)
                {
                    participant.SendHearingNotificationIfNew = true;
                }
            }
            
            var judiciaryParticipants = hearing.JudiciaryParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
            Participants.AddRange(judiciaryParticipants);

            var hearingEndpoints = hearing.GetEndpoints();
            Endpoints = hearingEndpoints.Select(he =>
                EndpointDtoMapper.MapToDto(he, hearing.GetParticipants(), hearing.GetEndpoints())).ToList();
            
            Func<Participant, bool> ParticipantHasContactEmail()
            {
                return x=> x.Person.ContactEmail is not null;
            }
        }

        public HearingDto Hearing { get; }
        public List<ParticipantDto> Participants { get; }
        public List<EndpointDto> Endpoints { get; set; }
    }
}

    