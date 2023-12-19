using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;
using System;

namespace BookingsApi.Infrastructure.Services
{
    public class EventDtoMappers
    {
        public static WelcomeEmailDto MapToWelcomeEmailDto(Guid hearingId, ParticipantDto participant, Case @case)
        {
            return new WelcomeEmailDto
            {
                HearingId = hearingId,
                CaseName = @case.Name,
                CaseNumber = @case.Number,
                ContactEmail = participant.GetContactEmail(),
                ContactTelephone = participant.GetContactTelephone(),
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ParticipnatId = participant.ParticipantId,
                UserRole = participant.UserRole
            };
        }

        public static HearingConfirmationForParticipantDto MapToHearingConfirmationDto(Guid hearingId, DateTime scheduledDateTime, ParticipantDto participant, Case @case)
        {
            return new HearingConfirmationForParticipantDto
            {
                HearingId = hearingId,
                ScheduledDateTime = scheduledDateTime,
                CaseName = @case.Name,
                CaseNumber = @case.Number,
                DisplayName = participant.DisplayName,
                Representee = participant.Representee,
                Username = participant.Username,
                ContactEmail = participant.GetContactEmail(),
                ContactTelephone = participant.GetContactTelephone(),
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ParticipnatId = participant.ParticipantId,
                UserRole = participant.UserRole
            };
        }

    }
}
