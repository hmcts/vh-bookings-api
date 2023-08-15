using System;
using BookingsApi.Validations;

namespace BookingsApi.UnitTests.Validation.V1
{
    public class EmailValidatorTests
    {
        [Test]
        public void Should_pass_validation_with_good_email()
        {
            var email = GetValidEmail();
            email.IsValidEmail().Should().BeTrue();
        }
        
        [Test]
        public void Should_fail_validation_when_empty()
        {
            var email = string.Empty;
            email.IsValidEmail().Should().BeFalse();
        }
        
        [Test]
        public void Should_fail_validation_when_format_is_invalid()
        {
            var email = GetInvalidEmail();
            email.IsValidEmail().Should().BeFalse();
        }

        public static string GetInvalidEmail()
        {
            const string firstName = "Automatically";
            const string lastName = "Created";
            var unique = DateTime.Now.ToString("yyyyMMddhmmss");
            var email = $"{firstName}.{lastName}.{unique}.@hearings.reform.hmcts.net";
            return email;
        }

        public static string GetValidEmail()
        {
            const string firstName = "Automatically";
            const string lastName = "Created";
            var unique = DateTime.Now.ToString("yyyyMMddhmmss");
            var email = $"{firstName}.{lastName}.{unique}@hearings.reform.hmcts.net";
            return email;
        }
    }
}