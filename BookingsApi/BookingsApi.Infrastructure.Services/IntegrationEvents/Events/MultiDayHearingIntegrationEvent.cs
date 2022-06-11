using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class MultiDayHearingIntegrationEvent : IIntegrationEvent
    {
        public MultiDayHearingIntegrationEvent(Hearing hearing, int totalDays)
        {
            Hearing = HearingDtoMapper.MapToDto(hearing);
            var hearingParticipants = hearing.GetParticipants();
            Participants = hearingParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
            TotalDays = totalDays;
        }

        public HearingDto Hearing { get; }
        public IList<ParticipantDto> Participants { get; }
        public int TotalDays { get; set; }
    }
}