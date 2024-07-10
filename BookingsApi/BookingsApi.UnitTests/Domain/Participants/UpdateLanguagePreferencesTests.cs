using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Participants;

public class UpdateLanguagePreferencesTests
{
    [Test]
    public void should_update_language()
    {
        var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);

        individualParticipant.UpdateLanguagePreferences(language, null);
        
        individualParticipant.InterpreterLanguage.Should().Be(language);
    }
    
    [Test]
    public void should_update_other_language()
    {
        var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
        var otherLanguage = "Other Language";

        individualParticipant.UpdateLanguagePreferences(null, otherLanguage);
        
        individualParticipant.OtherLanguage.Should().Be(otherLanguage);
    }
    
    [Test]
    public void should_throw_exception_when_validation_fails()
    {
        var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
        var language = new InterpreterLanguage(1, "spa", "Spanish", null, InterpreterType.Verbal, true);
        var otherLanguage = "Other Language";

        Action action = () => individualParticipant.UpdateLanguagePreferences(language, otherLanguage);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Name == "Participant" && x.Message == DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
    }
}