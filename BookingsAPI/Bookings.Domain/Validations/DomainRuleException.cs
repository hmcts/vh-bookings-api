using System;

namespace Bookings.Domain.Validations
{
    public class DomainRuleException : Exception
    {
        public DomainRuleException(ValidationFailures validationFailures)
        {
            ValidationFailures = validationFailures;
        }

        public DomainRuleException(string name, string message)
        {
            ValidationFailures.Add(new ValidationFailure(name, message));
        }

        public ValidationFailures ValidationFailures { get; protected set; } = new ValidationFailures();
    }
}