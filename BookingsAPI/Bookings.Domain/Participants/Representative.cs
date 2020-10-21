using System;
using System.Linq;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;

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

        public string Representee { get; set; }

        public void UpdateRepresentativeDetails(string representee)
        {
            ValidateArguments(representee);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }

            Representee = representee;
        }

        private void ValidateArguments(string representee)
        {
            if (string.IsNullOrEmpty(representee))
            {
                _validationFailures.AddFailure("Representee", "Representee is required");
            }
        }
    }
}