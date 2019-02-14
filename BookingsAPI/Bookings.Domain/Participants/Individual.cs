using Bookings.Domain.RefData;

namespace Bookings.Domain.Participants
{
    public class Individual : Participant
    {
        protected Individual()
        {
        }

        public Individual(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {

        }
    }
}