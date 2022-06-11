﻿using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class ParticipantsAddedIntegrationEvent: IIntegrationEvent
    {
        public ParticipantsAddedIntegrationEvent(Hearing hearing, IEnumerable<Participant> participants)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            Participants = participants.Select(participant => ParticipantDtoMapper.MapToDto(participant)).ToList();
        }

        public HearingDto Hearing { get; }

        public IList<ParticipantDto> Participants { get; }
    }
}