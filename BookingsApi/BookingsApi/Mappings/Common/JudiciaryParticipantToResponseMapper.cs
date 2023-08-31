using BookingsApi.Contract.V1.Responses;
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
                HearingRoleCode = MapHearingRoleCode(judiciaryParticipant.HearingRoleCode)
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
