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

        public string SolicitorsReference { get; set; }
        public string Representee { get; set; }

        public void UpdateRepresentativeDetails(string solicitorsReference, string representee)
        {
            ValidateArguments(solicitorsReference, representee);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }

            SolicitorsReference = solicitorsReference;
            Representee = representee;
        }

        private void ValidateArguments(string solicitorsReference, string representee)
        {
            if (string.IsNullOrEmpty(solicitorsReference))
            {
                _validationFailures.AddFailure("SolicitorsReference", "SolicitorsReference is required");
            }

            if (string.IsNullOrEmpty(representee))
            {
                _validationFailures.AddFailure("Representee", "Representee is required");
            }
        }
    }
}