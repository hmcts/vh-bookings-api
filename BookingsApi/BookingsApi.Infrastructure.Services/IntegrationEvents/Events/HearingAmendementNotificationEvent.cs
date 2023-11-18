using BookingsApi.Infrastructure.Services.Dtos;
using System;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingAmendementNotificationEvent : IIntegrationEvent
    {
        public HearingAmendementNotificationEvent(HearingConfirmationForParticipantDto dto, DateTime newScheduledDateTime)
        {
            HearingConfirmationForParticipant = dto;
            NewScheduledDateTime = newScheduledDateTime;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; }
        public DateTime NewScheduledDateTime { get; }
    }
}
