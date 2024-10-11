using System;
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

        [Obsolete("Use the constructor with the external reference id")]
        public Individual(Person person, HearingRole hearingRole, CaseRole caseRole) : base(person, hearingRole,
            caseRole)
        {

        }

        public Individual(string externalReferenceId, Person person, HearingRole hearingRole, string displayName) : base(
            externalReferenceId, person, hearingRole, displayName)
        {
        }

        protected override void ValidateParticipantDetails(string title, string displayName, string telephoneNumber, string organisationName)
        {
            ValidateArguments(displayName);

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }
    }
}