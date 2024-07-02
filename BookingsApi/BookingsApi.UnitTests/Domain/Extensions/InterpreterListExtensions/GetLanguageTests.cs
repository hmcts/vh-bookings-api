using System.Collections.Generic;
using BookingsApi.Domain.Extensions;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Extensions.InterpreterListExtensions
{
    public class GetLanguageTests
    {
        [Test]
        public void Should_get_language()
        {
            // Arrange
            const string languageCode = "spa";
            var languages = new List<InterpreterLanguage>
            {
                new(1, languageCode, "Spanish", null, InterpreterType.Verbal, true)
            };

            // Act
            var language = languages.GetLanguage(languageCode);

            // Assert
            language.Should().NotBeNull();
            language.Should().BeEquivalentTo(languages[0]);
        }

        [Test]
        public void Should_return_null_when_language_code_is_not_specified()
        {
            // Arrange
            var languages = new List<InterpreterLanguage>
            {
                new(1, "spa", "Spanish", null, InterpreterType.Verbal, true)
            };
            
            // Act
            var language = languages.GetLanguage("");

            // Assert
            language.Should().BeNull();
        }

        [TestCase("")]
        [TestCase("Endpoint")]
        public void Should_throw_exception_when_language_not_found(string errorKey)
        {
            // Arrange
            const string languageCode = "non existing";
            var languages = new List<InterpreterLanguage>
            {
                new(1, "spa", "Spanish", null, InterpreterType.Verbal, true)
            };
            
            // Act & Assert
            var action = () => 
                languages.GetLanguage(languageCode, errorKey);
            
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => 
                    x.Message == $"Language code {languageCode} does not exist" && x.Name == errorKey)
                .Should().BeTrue();
        }
    }
}
