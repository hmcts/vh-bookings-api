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

        public Individual(string externalReferenceId, Person person, HearingRole hearingRole, string displayName) : base(
            externalReferenceId, person, hearingRole, displayName)
        {
        }

        protected override void ValidateParticipantDetails(string title, string displayName, string telephoneNumber, string organisationName)
        {
            ValidateArguments(displayName);

            if (ValidationFailures.Any())
            {
                throw new DomainRuleException(ValidationFailures);
            }
        }
    }
}