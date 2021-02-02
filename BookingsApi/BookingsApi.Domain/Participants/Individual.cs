using System.Linq;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain.Participants
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