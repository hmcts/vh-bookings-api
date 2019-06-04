using Bookings.Domain.RefData;
using Bookings.Domain.Validations;

namespace Bookings.Domain.Participants
{
    public class Judge : Participant
    {
        protected Judge() { }

        public Judge(Person person, HearingRole hearingRole, CaseRole caseRole)
            : base(person, hearingRole, caseRole)
        {

        }

        public override void AddSuitabilityAnswer(string key, string data, string extendedData)
        {
            throw new DomainRuleException("SuitabilityAnswer", $"Cannot add suitability answers to Judge.");
        }
    }
}
