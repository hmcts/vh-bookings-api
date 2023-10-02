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

        [Test]
        public void should_not_pass_validation_when_email_has_ampersand()
        {
            var email = "test&more@foo.com";
            email.IsValidEmail().Should().BeFalse();
        }
        
        [Test]
        public void should_pass_validation_when_email_has_single_char_preceding_a_fullstop()
        {
            var email = "w.c@email.co.uk";
            email.IsValidEmail().Should().BeTrue();
        }        
        
        [TestCase("very.common@example.com")]
        [TestCase("x@example.com")]
        [TestCase("long.email-address-with-hyphens@and.subdomains.example.com")]
        [TestCase("name/surname@example.com")]
        public void should_pass_validation_with_all_valid_samples(string email)
        {
            email.IsValidEmail().Should().BeTrue();
        }
        
        [TestCase("abc.example.com")]
        [TestCase("a@b@c@example.com")]
        [TestCase($"this\\ still\\\"not\\\\allowed@example.com")]
        [TestCase("i.like.underscores@but_they_are_not_allowed_in_this_part")]
        public void should_fail_validation_with_all_invalid_samples(string email)
        {
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