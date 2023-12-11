using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;
using System;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Infrastructure.Services
{
    public class EventDtoMappers
    {
        public static WelcomeEmailDto MapToWelcomeEmailDto(Guid hearingId, Participant participant, Case @case)
        {
            return new WelcomeEmailDto
            {
                HearingId = hearingId,
                CaseName = @case.Name,
                CaseNumber = @case.Number,
                ContactEmail = participant.Person.ContactEmail,
                ContactTelephone = participant.Person.TelephoneNumber,
                FirstName = participant.Person.FirstName,
                LastName = participant.Person.LastName,
                ParticipnatId = participant.Id,
                UserRole = participant.HearingRole.UserRole.Name
            };
        }

        public static HearingConfirmationForParticipantDto MapToHearingConfirmationDto(Guid hearingId, DateTime scheduledDateTime, Participant participant, Case @case)
        {
            var representee = participant is Representative representative ? representative.Representee : string.Empty;
            return new HearingConfirmationForParticipantDto
            {
                HearingId = hearingId,
                ScheduledDateTime = scheduledDateTime,
                CaseName = @case.Name,
                CaseNumber = @case.Number,
                DisplayName = participant.DisplayName,
                Representee = representee,
                Username = participant.Person.Username,
                ContactEmail = participant.Person.ContactEmail,
                ContactTelephone = participant.Person.TelephoneNumber,
                FirstName = participant.Person.FirstName,
                LastName = participant.Person.LastName,
                ParticipnatId = participant.Id,
                UserRole = participant.HearingRole.UserRole.Name
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
                DisplayName = participant.DisplayName,
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
