using System;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain.Participants
{
    public class Judge : Participant
    {
        protected Judge() { }

        [Obsolete("Judges are now JudiciaryParticipants")]
        public Judge(Person person, HearingRole hearingRole, CaseRole caseRole)
            : base(person, hearingRole, caseRole)
        {
            DisplayName = $"{person.FirstName} {person.LastName}";
        }
    }
}
