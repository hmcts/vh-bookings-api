using System;
using System.Linq;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain.Participants
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
            ValidateArgumentsRepresentative(representee);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }

            Representee = representee;
        }

        private void ValidateArgumentsRepresentative(string representee)
        {
            if (string.IsNullOrEmpty(representee))
            {
                _validationFailures.AddFailure("Representee", "Representee is required");
            }
        }
    }
}