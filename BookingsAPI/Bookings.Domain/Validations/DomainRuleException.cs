using System;
using System.Diagnostics.CodeAnalysis;

namespace Bookings.Domain.Validations
{
    // ReSharper disable once S3925
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