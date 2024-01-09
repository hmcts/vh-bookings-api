using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;
using System;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Infrastructure.Services
{
    public static class EventDtoMappers
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
                DisplayName = string.IsNullOrEmpty(participant.DisplayName) ? $"{participant.FirstName} {participant.LastName}": participant.DisplayName,
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
        
        public static HearingConfirmationForParticipantDto MapToHearingConfirmationDto(Guid hearingId, DateTime scheduledDateTime, JudiciaryParticipant participant, Case @case)
        {
            return new HearingConfirmationForParticipantDto
            {
                HearingId = hearingId,
                ScheduledDateTime = scheduledDateTime,
                CaseName = @case.Name,
                CaseNumber = @case.Number,
                DisplayName = string.IsNullOrEmpty(participant.DisplayName) ? $"{participant.JudiciaryPerson.KnownAs} {participant.JudiciaryPerson.Surname}" : participant.DisplayName,
                Representee = "",
                Username = participant.JudiciaryPerson.Email, // we need to pass a username otherwise the notification is failing 
                ContactEmail = participant.JudiciaryPerson.Email,
                ContactTelephone = participant.JudiciaryPerson.WorkPhone,
                FirstName = participant.JudiciaryPerson.KnownAs,
                LastName = participant.JudiciaryPerson.Surname,
                ParticipnatId = participant.Id,
                UserRole =  GetUserRole(participant.HearingRoleCode)
            };
        }

        private static string GetUserRole(JudiciaryParticipantHearingRoleCode participantHearingRoleCode)
        {
            switch (participantHearingRoleCode)
            {
                case JudiciaryParticipantHearingRoleCode.Judge:
                    return "Judge";
                case JudiciaryParticipantHearingRoleCode.PanelMember:
                    return "Judicial Office Holder"; // Panel Member is a JudicialOfficeHolder Rolenames enum in Notification Api
                default:
                    throw new ArgumentOutOfRangeException(nameof(participantHearingRoleCode));
            }
        }
    }
}
