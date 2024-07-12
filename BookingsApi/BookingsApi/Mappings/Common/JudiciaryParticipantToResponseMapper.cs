using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using JudiciaryParticipantHearingRoleCode = BookingsApi.Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode;

namespace BookingsApi.Mappings.Common
{
    public class JudiciaryParticipantToResponseMapper
    {
        public JudiciaryParticipantResponse MapJudiciaryParticipantToResponse(JudiciaryParticipant judiciaryParticipant)
        {
            var response = new JudiciaryParticipantResponse
            {
                PersonalCode = judiciaryParticipant.JudiciaryPerson.PersonalCode,
                DisplayName = judiciaryParticipant.DisplayName,
                HearingRoleCode = MapHearingRoleCode(judiciaryParticipant.HearingRoleCode),
                Email = judiciaryParticipant.JudiciaryPerson.Email,
                Title = judiciaryParticipant.JudiciaryPerson.Title,
                FirstName = judiciaryParticipant.JudiciaryPerson.KnownAs,
                LastName = judiciaryParticipant.JudiciaryPerson.Surname,
                FullName = judiciaryParticipant.JudiciaryPerson.Fullname,
                WorkPhone = judiciaryParticipant.JudiciaryPerson.WorkPhone,
                IsGeneric = judiciaryParticipant.JudiciaryPerson.IsGeneric,
                OptionalContactEmail = judiciaryParticipant.GetEmail(),
                OptionalContactTelephone = judiciaryParticipant.GetTelephone(),
                InterpreterLanguage = judiciaryParticipant.InterpreterLanguage != null ? 
                    InterpreterLanguageToResponseMapper.MapInterpreterLanguageToResponse(judiciaryParticipant.InterpreterLanguage) : 
                    null,
                OtherLanguage = judiciaryParticipant.OtherLanguage
            };

            return response;
        }
        
        private static JudiciaryParticipantHearingRoleCode MapHearingRoleCode(Domain.Enumerations.JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            return hearingRoleCode switch
            {
                Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge => JudiciaryParticipantHearingRoleCode.Judge,
                Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember => JudiciaryParticipantHearingRoleCode.PanelMember,
                _ => throw new ArgumentOutOfRangeException(nameof(hearingRoleCode), hearingRoleCode, null)
            };
        }
    }
}
