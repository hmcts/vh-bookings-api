using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain.Participants
{
    public class StaffMember : Participant
    {
        protected StaffMember() { }

        public StaffMember(Person person, HearingRole hearingRole, CaseRole caseRole)
            : base(person, hearingRole, caseRole)
        {
        }
    }
}
