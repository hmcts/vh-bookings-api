using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingDateTimeChangedIntegrationEvent : IIntegrationEvent
    {
        public HearingDateTimeChangedIntegrationEvent(Hearing hearing, DateTime oldScheduledDateTime)
        {
            OldScheduledDateTime = oldScheduledDateTime;
            Hearing = HearingDtoMapper.MapToDto(hearing);
            var hearingParticipants = hearing.GetParticipants();
            Participants = hearingParticipants.Select(ParticipantDtoMapper.MapToDto).ToList();
            Participants.ToList().ForEach(x =>
                x.SetContactEmailForNonEJudJudgeUser(hearing.OtherInformation));

        }

        public HearingDto Hearing { get; }
        public DateTime OldScheduledDateTime { get; }
        public IList<ParticipantDto> Participants { get; }
    }
}