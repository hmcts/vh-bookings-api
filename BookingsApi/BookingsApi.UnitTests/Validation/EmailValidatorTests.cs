using BookingsApi.Validations;
using Faker;
using FluentAssertions;
using NUnit.Framework;
using System;
using Moq;
using System.Text.RegularExpressions;

namespace BookingsApi.UnitTests.Validation
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

        public string GetInvalidEmail()
        {
            const string firstName = "Automatically";
            const string lastName = "Created";
            var unique = DateTime.Now.ToString("yyyyMMddhmmss");
            var email = $"{firstName}.{lastName}.{unique}.@hearings.reform.hmcts.net";
            return email;
        }

        public string GetValidEmail()
        {
            const string firstName = "Automatically";
            const string lastName = "Created";
            var unique = DateTime.Now.ToString("yyyyMMddhmmss");
            var email = $"{firstName}.{lastName}.{unique}@hearings.reform.hmcts.net";
            return email;
        }
    }
}