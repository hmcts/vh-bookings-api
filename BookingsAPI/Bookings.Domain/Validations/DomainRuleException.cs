using System;
using System.Linq;

namespace Bookings.Domain.Validations
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class DomainRuleException : Exception
    {
        public DomainRuleException(ValidationFailures validationFailures) : base(BuildMessage(validationFailures))
        {
            ValidationFailures = validationFailures;
        }

        private static string BuildMessage(ValidationFailures validationFailures)
        {
            if (validationFailures.Count == 1)
            {
                return validationFailures.First().Message;
            }

            var errors = string.Join(", ", validationFailures.Select(v => v.Message));
            return $"Domain validation failed with errors: {errors}";
        }

        public DomainRuleException(string name, string message) : base(message)
        {
            ValidationFailures.Add(new ValidationFailure(name, message));
        }

        public ValidationFailures ValidationFailures { get; protected set; } = new ValidationFailures();
    }
}