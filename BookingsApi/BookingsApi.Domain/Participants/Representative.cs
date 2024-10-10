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

        [Obsolete("Use the constructor with the external reference id")]
        public Representative(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {
        }
        
        public Representative(string externalReferenceId, Person person, HearingRole hearingRole, string displayName,
            string representee) : base(
            externalReferenceId, person, hearingRole, displayName)
        {
            Representee = representee;
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