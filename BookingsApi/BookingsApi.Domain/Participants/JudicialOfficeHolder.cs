using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain.Participants
{
    public class JudicialOfficeHolder : Participant
    {
        protected JudicialOfficeHolder() { }
        
        public JudicialOfficeHolder(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {
        }
    }
}
