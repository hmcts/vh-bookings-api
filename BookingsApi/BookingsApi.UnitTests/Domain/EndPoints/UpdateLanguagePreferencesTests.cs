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
        _endpoint = new Endpoint(Guid.NewGuid().ToString(),"Original DisplayName", "sip@videohearings.net", "1234");
    }
        
    [Test]
    public void should_update_language()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        _endpoint.UpdateLanguagePreferences(language, null);
        
        _endpoint.InterpreterLanguage.Should().Be(language);
        _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }
    
    [Test]
    public void should_update_other_language()
    {
        const string otherLanguage = "Other Language";
        _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        _endpoint.UpdateLanguagePreferences(null, otherLanguage);
        
        _endpoint.OtherLanguage.Should().Be(otherLanguage);
        _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
    }
    
    [Test]
    public void should_throw_exception_when_validation_fails()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        const string otherLanguage = "Other Language";

        var action = () => _endpoint.UpdateLanguagePreferences(language, otherLanguage);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Name == "Endpoint" && x.Message == DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
    }

    [Test]
    public void should_not_update_language_when_not_changed()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        _endpoint.UpdateLanguagePreferences(language, null);
        _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        _endpoint.UpdateLanguagePreferences(language, null);

        _endpoint.InterpreterLanguage.Should().Be(language);
        _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
    }
    
    [Test]
    public void should_not_update_other_language_when_not_changed()
    {
        const string otherLanguage = "other-language";
        _endpoint.UpdateLanguagePreferences(null, otherLanguage);
        _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
        var originalUpdatedDate = _endpoint.UpdatedDate;
        
        _endpoint.UpdateLanguagePreferences(null, otherLanguage);

        _endpoint.OtherLanguage.Should().Be(otherLanguage);
        _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
    }
}