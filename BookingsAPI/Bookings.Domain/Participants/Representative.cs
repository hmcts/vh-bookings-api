using System;
using System.Linq;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;

namespace Bookings.Domain.Participants
{
    public class Representative : Participant
    {
        public static Representative CreateRepresentativeWithId(Guid id)
        {
            return new Representative {Id = id};
        }
        protected Representative()
        {
        }

        public Representative(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {
        }

        public string Reference { get; set; }
        public string Representee { get; set; }

        public void UpdateRepresentativeDetails(string reference, string representee)
        {
            ValidateArguments(reference, representee);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }

            Reference = reference;
            Representee = representee;
        }

        private void ValidateArguments(string reference, string representee)
        {
            if (string.IsNullOrEmpty(reference))
            {
                _validationFailures.AddFailure("Reference", "Reference is required");
            }

            if (string.IsNullOrEmpty(representee))
            {
                _validationFailures.AddFailure("Representee", "Representee is required");
            }
        }
    }
}