using Bookings.Domain.RefData;

namespace Bookings.Domain.Participants
{
    public class JudicialOfficeHolder : Participant
    {
        public JudicialOfficeHolder(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {
        }
    }
}
