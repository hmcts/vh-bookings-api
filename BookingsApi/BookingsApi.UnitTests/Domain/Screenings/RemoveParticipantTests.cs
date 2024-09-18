using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class RemoveParticipantTests
{
    [Test]
    public void should_remove_participant_from_screening()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var screenFrom = new ParticipantBuilder().IndividualParticipantRespondent;
        
        screening.AddParticipant(screenFrom);
        screening.RemoveParticipant(screenFrom);
        
        screening.GetParticipants().Should().NotContain(x => x.Participant == screenFrom);
    }
    
    [Test]
    public void should_throw_exception_when_participant_does_not_exist()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var screenFrom = new ParticipantBuilder().IndividualParticipantRespondent;
        
        Action action = () => screening.RemoveParticipant(screenFrom);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message == DomainRuleErrorMessages.ParticipantDoesNotExistForScreening);
    }
}