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
        [TestCase("x.x@example.com")]
        [TestCase("long.email-address-with-hyphens@and.subdomains.example.com")]
        [TestCase("name/surname@example.com")]
        public void should_pass_validation_with_all_valid_samples(string email)
        {
            email.IsValidEmail().Should().BeTrue();
        }
        
        [TestCase("abc.example.com")]
        [TestCase("x.@example.com")]
        [TestCase("x.x.@example.com")]
        [TestCase("a@b@c@example.com")]
        [TestCase($"this\\ still\\\"not\\\\allowed@example.com")]
        [TestCase("i.like.underscores@but_they_are_not_allowed_in_this_part")]
        public void should_fail_validation_with_all_invalid_samples(string email)
        {
            email.IsValidEmail().Should().BeFalse();
        } 

        [TestCase("Áá@créâtïvéàççénts.com")]
        [TestCase("Àà@créâtïvéàççénts.com")]
        [TestCase("Ââ@créâtïvéàççénts.com")]
        [TestCase("Ää@créâtïvéàççénts.com")]
        [TestCase("Ãã@créâtïvéàççénts.com")]
        [TestCase("Åå@créâtïvéàççénts.com")]
        [TestCase("Āā@créâtïvéàççénts.com")]
        [TestCase("Ăă@créâtïvéàççénts.com")]
        [TestCase("Ąą@créâtïvéàççénts.com")]
        [TestCase("Ćć@créâtïvéàççénts.com")]
        [TestCase("Ĉĉ@créâtïvéàççénts.com")]
        [TestCase("Ċċ@créâtïvéàççénts.com")]
        [TestCase("Čč@créâtïvéàççénts.com")]
        [TestCase("Ðð@créâtïvéàççénts.com")]
        [TestCase("Éé@créâtïvéàççénts.com")]
        [TestCase("Èè@créâtïvéàççénts.com")]
        [TestCase("Êê@créâtïvéàççénts.com")]
        [TestCase("Ëë@créâtïvéàççénts.com")]
        [TestCase("Ēē@créâtïvéàççénts.com")]
        [TestCase("Ĕĕ@créâtïvéàççénts.com")]
        [TestCase("Ėė@créâtïvéàççénts.com")]
        [TestCase("Ęę@créâtïvéàççénts.com")]
        [TestCase("Ěě@créâtïvéàççénts.com")]
        [TestCase("Ĝĝ@créâtïvéàççénts.com")]
        [TestCase("Ğğ@créâtïvéàççénts.com")]
        [TestCase("Ġġ@créâtïvéàççénts.com")]
        [TestCase("Ģģ@créâtïvéàççénts.com")]
        [TestCase("Ĥĥ@créâtïvéàççénts.com")]
        [TestCase("Ħħ@créâtïvéàççénts.com")]
        [TestCase("Íí@créâtïvéàççénts.com")]
        [TestCase("Ìì@créâtïvéàççénts.com")]
        [TestCase("Îî@créâtïvéàççénts.com")]
        [TestCase("Ïï@créâtïvéàççénts.com")]
        [TestCase("Ĩĩ@créâtïvéàççénts.com")]
        [TestCase("Īī@créâtïvéàççénts.com")]
        [TestCase("Ĭĭ@créâtïvéàççénts.com")]
        [TestCase("Įį@créâtïvéàççénts.com")]
        [TestCase("İı@créâtïvéàççénts.com")]
        [TestCase("Ĵĵ@créâtïvéàççénts.com")]
        [TestCase("Ķķ@créâtïvéàççénts.com")]
        [TestCase("Ĺĺ@créâtïvéàççénts.com")]
        [TestCase("Ļļ@créâtïvéàççénts.com")]
        [TestCase("Ľľ@créâtïvéàççénts.com")]
        [TestCase("Łł@créâtïvéàççénts.com")]
        [TestCase("Ńń@créâtïvéàççénts.com")]
        [TestCase("Ņņ@créâtïvéàççénts.com")]
        [TestCase("Ňň@créâtïvéàççénts.com")]
        [TestCase("Ññ@créâtïvéàççénts.com")]
        [TestCase("Óó@créâtïvéàççénts.com")]
        [TestCase("Òò@créâtïvéàççénts.com")]
        [TestCase("Ôô@créâtïvéàççénts.com")]
        [TestCase("Öö@créâtïvéàççénts.com")]
        [TestCase("Õõ@créâtïvéàççénts.com")]
        [TestCase("Øø@créâtïvéàççénts.com")]
        [TestCase("Ōō@créâtïvéàççénts.com")]
        [TestCase("Ŏŏ@créâtïvéàççénts.com")]
        [TestCase("Őő@créâtïvéàççénts.com")]
        [TestCase("Ŕŕ@créâtïvéàççénts.com")]
        [TestCase("Ŗŗ@créâtïvéàççénts.com")]
        [TestCase("Řř@créâtïvéàççénts.com")]
        [TestCase("Śś@créâtïvéàççénts.com")]
        [TestCase("Ŝŝ@créâtïvéàççénts.com")]
        [TestCase("Şş@créâtïvéàççénts.com")]
        [TestCase("Šš@créâtïvéàççénts.com")]
        [TestCase("Ţţ@créâtïvéàççénts.com")]
        [TestCase("Ťť@créâtïvéàççénts.com")]
        [TestCase("Ŧŧ@créâtïvéàççénts.com")]
        [TestCase("Úú@créâtïvéàççénts.com")]
        [TestCase("Ùù@créâtïvéàççénts.com")]
        [TestCase("Ûû@créâtïvéàççénts.com")]
        [TestCase("Üü@créâtïvéàççénts.com")]
        [TestCase("Ũũ@créâtïvéàççénts.com")]
        [TestCase("Ūū@créâtïvéàççénts.com")]
        [TestCase("Ŭŭ@créâtïvéàççénts.com")]
        [TestCase("Ůů@créâtïvéàççénts.com")]
        [TestCase("Űű@créâtïvéàççénts.com")]
        [TestCase("Ųų@créâtïvéàççénts.com")]
        [TestCase("Ŵŵ@créâtïvéàççénts.com")]
        [TestCase("Ẁẁ@créâtïvéàççénts.com")]
        [TestCase("Ẃẃ@créâtïvéàççénts.com")]
        [TestCase("Ẅẅ@créâtïvéàççénts.com")]
        [TestCase("Ýý@créâtïvéàççénts.com")]
        [TestCase("Ỳỳ@créâtïvéàççénts.com")]
        [TestCase("Ŷŷ@créâtïvéàççénts.com")]
        [TestCase("Ÿÿ@créâtïvéàççénts.com")]
        [TestCase("Źź@créâtïvéàççénts.com")]
        [TestCase("Żż@créâtïvéàççénts.com")]
        [TestCase("Žž@créâtïvéàççénts.com")]
        public void Should_pass_validation_when_email_contains_accented_characters(string email)
        {
            email.IsValidEmail().Should().BeTrue();
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