using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using System.Linq;

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

        protected override void ValidatePartipantDetails(string title, string displayName, string telephoneNumber, string organisationName)
        {
            ValidateArguments(displayName);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }
    }
}