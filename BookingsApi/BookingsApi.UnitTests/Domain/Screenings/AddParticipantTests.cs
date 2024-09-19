using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class AddParticipantTests
{
    [Test]
    public void should_add_participant_to_screening()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screenFrom = new ParticipantBuilder().IndividualParticipantRespondent;
        var screening = new Screening(ScreeningType.Specific, participant);
        
        screening.AddParticipant(screenFrom);
        
        screening.GetParticipants().Should().Contain(x => x.Participant == screenFrom);
    }
    
    [Test]
    public void should_throw_exception_when_participant_already_added()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var screenFrom = new ParticipantBuilder().IndividualParticipantRespondent;
        
        screening.AddParticipant(screenFrom);
        
        Action action = () => screening.AddParticipant(screenFrom);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message == DomainRuleErrorMessages.ParticipantAlreadyAddedForScreening);
    }
}