using System;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain.Participants
{
    public class JudicialOfficeHolder : Participant
    {
        protected JudicialOfficeHolder() { }
        
        [Obsolete("JudicialOfficeHolder are now JudiciaryParticipants")]
        public JudicialOfficeHolder(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {
        }
    }
}
