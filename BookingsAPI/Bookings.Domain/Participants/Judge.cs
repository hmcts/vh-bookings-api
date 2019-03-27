using Bookings.Domain.RefData;

namespace Bookings.Domain.Participants
{
    public class Judge : Participant
    {
        protected Judge() { }

        public Judge(Person person, HearingRole hearingRole, CaseRole caseRole)
            : base(person, hearingRole, caseRole)
        {

        }
    }
}
