using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.EndPoints;

public class UpdateLanguagePreferencesTests : DomainTests
{
    private Endpoint _endpoint;

    [SetUp]
    public void Setup()
    {
        _endpoint = new Endpoint(Guid.NewGuid().ToString(),"Original DisplayName", "sip@videohearings.net", "1234", null);
    }
        
    [Test]
    public async Task should_update_language()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        var originalUpdatedDate = DateTime.UtcNow;

        await ApplyDelay();
        _endpoint.UpdateLanguagePreferences(language, null);
        
        _endpoint.InterpreterLanguage.Should().Be(language);
        _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }
    
    [Test]
    public async Task should_update_other_language()
    {
        var otherLanguage = "Other Language";
        var originalUpdatedDate = DateTime.UtcNow;

        await ApplyDelay();
        _endpoint.UpdateLanguagePreferences(null, otherLanguage);
        
        _endpoint.OtherLanguage.Should().Be(otherLanguage);
        _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
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

    [Test]
    public async Task should_not_update_language_when_not_changed()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        _endpoint.UpdateLanguagePreferences(language, null);
        var originalUpdatedDate = _endpoint.UpdatedDate;

        await ApplyDelay();
        _endpoint.UpdateLanguagePreferences(language, null);

        _endpoint.InterpreterLanguage.Should().Be(language);
        _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
    }
    
    [Test]
    public async Task should_not_update_other_language_when_not_changed()
    {
        var otherLanguage = "other-language";
        _endpoint.UpdateLanguagePreferences(null, otherLanguage);
        var originalUpdatedDate = _endpoint.UpdatedDate;

        await ApplyDelay();
        _endpoint.UpdateLanguagePreferences(null, otherLanguage);

        _endpoint.OtherLanguage.Should().Be(otherLanguage);
        _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
    }
}