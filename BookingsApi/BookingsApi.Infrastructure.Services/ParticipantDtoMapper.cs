using System;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class ParticipantDtoMapper
    {
        public static ParticipantDto MapToDto(Participant participant)
        {
            var representee = participant is Representative representative ? representative.Representee : string.Empty;
            return
                new ParticipantDto
                {
                    ParticipantId = participant.Id,
                    Fullname = $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}",
                    Username = participant.Person.Username,
                    FirstName = participant.Person.FirstName,
                    LastName = participant.Person.LastName,
                    ContactEmail = participant.Person.ContactEmail,
                    ContactTelephone = participant.Person.TelephoneNumber,
                    DisplayName = participant.DisplayName,
                    HearingRole = participant.HearingRole.Name,
                    UserRole = participant.HearingRole.UserRole.Name,
                    CaseGroupType = participant.CaseRole.Group,
                    Representee = representee,
                    LinkedParticipants = participant.LinkedParticipants.Select(LinkedParticipantDtoMapper.MapToDto).ToList()
                };
        }
        
        public static ParticipantDto MapToDto(JudiciaryParticipant judiciaryParticipant) =>
            new()
            {
                ParticipantId = judiciaryParticipant.Id,
                Fullname = $"{judiciaryParticipant.JudiciaryPerson.Fullname}",
                Username = judiciaryParticipant.JudiciaryPerson.Email,
                FirstName = judiciaryParticipant.JudiciaryPerson.KnownAs,
                LastName = judiciaryParticipant.JudiciaryPerson.Surname,
                ContactEmail = judiciaryParticipant.JudiciaryPerson.Email,
                DisplayName = judiciaryParticipant.DisplayName,
                HearingRole = MapHearingRoleForJudiciaryParticipant(judiciaryParticipant.HearingRoleCode),
                UserRole = MapUserRoleForJudiciaryParticipant(judiciaryParticipant.HearingRoleCode),
                CaseGroupType = MapCaseGroupTypeForJudiciaryParticipant(judiciaryParticipant.HearingRoleCode)
            };

        private static string MapHearingRoleForJudiciaryParticipant(JudiciaryParticipantHearingRoleCode hearingRoleCode) =>
            hearingRoleCode switch
            {
                JudiciaryParticipantHearingRoleCode.Judge => "Judge",
                JudiciaryParticipantHearingRoleCode.PanelMember => "Panel Member",
                _ => throw new ArgumentOutOfRangeException(nameof(hearingRoleCode), hearingRoleCode, null)
            };
        
        private static string MapUserRoleForJudiciaryParticipant(JudiciaryParticipantHearingRoleCode hearingRoleCode) =>
            hearingRoleCode switch
            {
                JudiciaryParticipantHearingRoleCode.Judge => "Judge",
                JudiciaryParticipantHearingRoleCode.PanelMember => "Judicial Office Holder",
                _ => throw new ArgumentOutOfRangeException(nameof(hearingRoleCode), hearingRoleCode, null)
            };

        private static CaseRoleGroup MapCaseGroupTypeForJudiciaryParticipant(JudiciaryParticipantHearingRoleCode hearingRoleCode) =>
            hearingRoleCode switch
            {
                JudiciaryParticipantHearingRoleCode.Judge => CaseRoleGroup.Judge,
                JudiciaryParticipantHearingRoleCode.PanelMember => CaseRoleGroup.PanelMember,
                _ => throw new ArgumentOutOfRangeException(nameof(hearingRoleCode), hearingRoleCode, null)
            };
    }
}