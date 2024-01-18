using System.Linq;
using System;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class ParticipantDtoMapper
    {
        public static ParticipantDto MapToDto(Participant participant)
        {
            var representee = participant is Representative representative ? representative.Representee : string.Empty;
            var caseGroupType = participant.CaseRole?.Group ??
                                InferCaseRoleGroupFromHearingRole(participant.HearingRole);
            return
                new ParticipantDto
                {
                    ParticipantId = participant.Id,
                    Fullname =
                        $"{participant.Person.Title} {participant.Person.FirstName} {participant.Person.LastName}".Trim(),
                    Username = participant.Person.Username,
                    FirstName = participant.Person.FirstName,
                    LastName = participant.Person.LastName,
                    ContactEmail = participant.Person.ContactEmail,
                    ContactTelephone = participant.Person.TelephoneNumber,
                    DisplayName = participant.DisplayName,
                    HearingRole = participant.HearingRole.Name,
                    UserRole = participant.HearingRole.UserRole.Name,
                    CaseGroupType = caseGroupType,
                    Representee = representee,
                    LinkedParticipants = participant.LinkedParticipants.Select(LinkedParticipantDtoMapper.MapToDto)
                        .ToList()
                };
        }

        public static ParticipantDto MapToDto(Participant participant, string otherInformation)
        {
            var participantDto = MapToDto(participant);

            if (participant is Judge)
            {
                participantDto.SetOtherFieldsForNonEJudJudgeUser(otherInformation);
            }

            return participantDto;
        }

        public static ParticipantDto MapToDto(JudiciaryParticipant judiciaryParticipant) =>
            new()
            {
                ParticipantId = judiciaryParticipant.Id,
                Fullname = $"{judiciaryParticipant.JudiciaryPerson.Fullname}",
                Username = judiciaryParticipant.JudiciaryPerson.Email,
                FirstName = judiciaryParticipant.JudiciaryPerson.KnownAs,
                LastName = judiciaryParticipant.JudiciaryPerson.Surname,
                ContactEmail = judiciaryParticipant.GetEmail(),
                ContactTelephone = judiciaryParticipant.GetTelephone(),
                DisplayName = judiciaryParticipant.DisplayName,
                HearingRole = MapHearingRoleForJudiciaryParticipant(judiciaryParticipant.HearingRoleCode),
                UserRole = MapUserRoleForJudiciaryParticipant(judiciaryParticipant.HearingRoleCode),
                CaseGroupType = MapCaseGroupTypeForJudiciaryParticipant(judiciaryParticipant.HearingRoleCode)
            };

        /// <summary>
        /// The flat hearing role structure means participants do not have a case role group, so we infer it from the hearing role
        /// </summary>
        /// <param name="hearingRole"></param>
        /// <returns>Case role group</returns>
        private static CaseRoleGroup InferCaseRoleGroupFromHearingRole(HearingRole hearingRole)
        {
            if (hearingRole.UserRole.IsJudge)
            {
                return CaseRoleGroup.Judge;
            }

            if (hearingRole.UserRole.IsJudicialOfficeHolder)
            {
                return CaseRoleGroup.PanelMember;
            }

            if (hearingRole.Name == "Observer")
            {
                return CaseRoleGroup.Observer;
            }

            return CaseRoleGroup.None;
        }



        private static string
            MapHearingRoleForJudiciaryParticipant(JudiciaryParticipantHearingRoleCode hearingRoleCode) =>
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

        private static CaseRoleGroup MapCaseGroupTypeForJudiciaryParticipant(
            JudiciaryParticipantHearingRoleCode hearingRoleCode) =>
            hearingRoleCode switch
            {
                JudiciaryParticipantHearingRoleCode.Judge => CaseRoleGroup.Judge,
                JudiciaryParticipantHearingRoleCode.PanelMember => CaseRoleGroup.PanelMember,
                _ => throw new ArgumentOutOfRangeException(nameof(hearingRoleCode), hearingRoleCode, null)
            };
    }
}