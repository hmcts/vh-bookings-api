using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain.Participants
{
    public class Judge : Participant
    {
        protected Judge() { }

        public Judge(Person person, HearingRole hearingRole, CaseRole caseRole)
            : base(person, hearingRole, caseRole)
        {
            DisplayName = $"{person.FirstName} {person.LastName}";
        }
    }
}
