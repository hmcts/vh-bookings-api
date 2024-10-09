using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing;

public class AssignScreeningForParticipantTests
{
    [Test]
    public void should_throw_exception_if_screening_participant_from_themself()
    {
        var hearing = new VideoHearingBuilder().Build();
        var participant = hearing.Participants.First(x => x is Individual);
        
        Action action = () => hearing.AssignScreeningForParticipant(participant, ScreeningType.Specific, [participant.Person.ContactEmail], []);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message.Contains(DomainRuleErrorMessages.ParticipantCannotScreenFromThemself));
    }
    
    [Test]
    public void should_throw_exception_if_screening_from_participant_does_not_exist()
    {
        var hearing = new VideoHearingBuilder().Build();
        var participant = hearing.Participants.First(x => x is Individual);
        
        Action action = () => hearing.AssignScreeningForParticipant(participant, ScreeningType.Specific, ["does-not-exist"], []);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message.Contains(DomainRuleErrorMessages.ParticipantDoesNotExist));
    }

    [Test]
    public void should_throw_exception_if_screen_from_endpoint_does_not_exist()
    {
        var hearing = new VideoHearingBuilder().Build();
        var participant = hearing.Participants.First(x => x is Individual);
        
        Action action = () => hearing.AssignScreeningForParticipant(participant, ScreeningType.Specific, [], ["does-not-exist"]);
        // should throw a DomainRuleException with the message contains DomainRuleErrorMessages.ParticipantDoesNotExist
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message.Contains(DomainRuleErrorMessages.EndpointDoesNotExist));
    }
}