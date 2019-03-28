using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Domain
{
    public class DomainRuleExceptionTests
    {
        [Test]
        public void should_set_message()
        {
            new DomainRuleException("Role", "Role not recognised").Message
                .Should().Be("Role not recognised");
        }
        
        [Test]
        public void should_concatenate_multiple_validation_errors_as_message()
        {
            var errors = new ValidationFailures();
            errors.AddFailure("Role", "Role not recognised");
            errors.AddFailure("Name", "Name should not be empty");
            var exception = new DomainRuleException(errors);
            exception.Message.Should().Be("Domain validation failed with errors: Role not recognised, Name should not be empty");
        }

        [Test]
        public void should_pick_single_validation_error_as_message_if_only_one()
        {
            var errors = new ValidationFailures();
            errors.AddFailure("name", "Name should not be empty");
            new DomainRuleException(errors).Message.Should().Be("Name should not be empty");
        }
    }
}