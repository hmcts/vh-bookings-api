using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.EndPoints;

public class UpdateLanguagePreferencesTests
{
    private Endpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _endpoint = new Endpoint(Guid.NewGuid().ToString(),"Original DisplayName", "sip@videohearings.net", "1234", null);
    }
        
    [Test]
    public void should_update_language()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);

        _endpoint.UpdateLanguagePreferences(language, null);
        
        _endpoint.InterpreterLanguage.Should().Be(language);
    }
    
    [Test]
    public void should_update_other_language()
    {
        var otherLanguage = "Other Language";

        _endpoint.UpdateLanguagePreferences(null, otherLanguage);
        
        _endpoint.OtherLanguage.Should().Be(otherLanguage);
    }
    
    [Test]
    public void should_throw_exception_when_validation_fails()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        var otherLanguage = "Other Language";

        Action action = () => _endpoint.UpdateLanguagePreferences(language, otherLanguage);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Name == "Endpoint" && x.Message == DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
    }
}