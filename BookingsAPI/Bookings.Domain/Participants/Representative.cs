using Bookings.Domain.RefData;

namespace Bookings.Domain.Participants
{
    public class Representative : Participant
    {
        protected Representative()
        {
        }

        public Representative(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {
        }

        public string SolicitorsReference { get; set; }
        public string Representee { get; set; }
    }
}