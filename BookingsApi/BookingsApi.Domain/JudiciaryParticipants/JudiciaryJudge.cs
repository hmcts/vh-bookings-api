using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain.JudiciaryParticipants
{
    public class JudiciaryJudge : JudiciaryParticipant
    {
        public JudiciaryJudge(string displayName, JudiciaryPerson judiciaryPerson, string contactEmail = null) 
            : base(displayName, judiciaryPerson, JudiciaryParticipantHearingRoleCode.Judge, optionalContactEmail: contactEmail)
        {
        }
    }
}
