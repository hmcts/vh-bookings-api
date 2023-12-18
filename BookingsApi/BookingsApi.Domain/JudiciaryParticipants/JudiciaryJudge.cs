using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain.JudiciaryParticipants
{
    public class JudiciaryJudge : JudiciaryParticipant
    {
        public JudiciaryJudge(string displayName, JudiciaryPerson judiciaryPerson) 
            : base(displayName, judiciaryPerson, JudiciaryParticipantHearingRoleCode.Judge)
        {
        }
    }
}
