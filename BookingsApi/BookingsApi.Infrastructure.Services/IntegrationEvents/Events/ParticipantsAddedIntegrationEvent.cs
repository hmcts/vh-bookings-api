﻿using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantsAddedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantsAddedIntegrationEvent(Guid hearingId, IEnumerable<Participant> participants)
        { 
            Participants = participants.Select(participant => ParticipantDtoMapper.MapToDto(participant)).ToList();
            HearingId = hearingId;
        }

        public Guid HearingId { get; }

        public IList<ParticipantDto> Participants { get; }
    }
}