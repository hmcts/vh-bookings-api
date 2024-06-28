using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.JudiciaryParticipants;

public class UpdateLanguagePreferencesTests
{
    private BookingsApi.Domain.JudiciaryParticipant _judiciaryParticipant;

    [SetUp]
    public void Setup()
    {
        var newJudiciaryPerson = new JudiciaryPersonBuilder().Build();
        _judiciaryParticipant =
            new BookingsApi.Domain.JudiciaryParticipant("Judge", newJudiciaryPerson,
                JudiciaryParticipantHearingRoleCode.Judge, null, null);
    }
    
    [Test]
    public void should_update_language()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);

        _judiciaryParticipant.UpdateLanguagePreferences(language, null);
        
        _judiciaryParticipant.InterpreterLanguage.Should().Be(language);
    }
    
    [Test]
    public void should_update_other_language()
    {
        var otherLanguage = "Other Language";

        _judiciaryParticipant.UpdateLanguagePreferences(null, otherLanguage);
        
        _judiciaryParticipant.OtherLanguage.Should().Be(otherLanguage);
    }
    
    [Test]
    public void should_throw_exception_when_validation_fails()
    {
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        var otherLanguage = "Other Language";

        Action action = () => _judiciaryParticipant.UpdateLanguagePreferences(language, otherLanguage);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Name == "JudiciaryParticipant" && x.Message == DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
    }
}