﻿using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class NewParticipantMultidayHearingConfirmationEvent : IIntegrationEvent
    {
        public NewParticipantMultidayHearingConfirmationEvent(HearingConfirmationForParticipantDto dto, int totalDays)
        {
            HearingConfirmationForParticipant = dto;
            TotalDays = totalDays;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; }
        public int TotalDays { get; }
    }
}
