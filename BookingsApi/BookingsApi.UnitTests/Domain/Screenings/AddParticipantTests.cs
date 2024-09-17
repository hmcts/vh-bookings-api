using BookingsApi.Domain;
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

public class AddEndpointTests
{
    [Test]
    public void should_add_endpoint_to_screening()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var endpoint = new Endpoint("name", "sip", "pin", null);
        
        screening.AddEndpoint(endpoint);
        
        screening.GetEndpoints().Should().Contain(x => x.Endpoint == endpoint);
    }
    
    [Test]
    public void should_throw_exception_when_endpoint_already_added()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var endpoint = new Endpoint("name", "sip", "pin", null);
        
        screening.AddEndpoint(endpoint);
        
        Action action = () => screening.AddEndpoint(endpoint);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message == DomainRuleErrorMessages.EndpointAlreadyAddedForScreening);
    }
}

public class RemoveEndpointTests
{
    [Test]
    public void should_remove_endpoint_from_screening()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var endpoint = new Endpoint("name", "sip", "pin", null);
        
        screening.AddEndpoint(endpoint);
        screening.RemoveEndpoint(endpoint);
        
        screening.GetEndpoints().Should().NotContain(x => x.Endpoint == endpoint);
    }
    
    [Test]
    public void should_throw_exception_when_endpoint_does_not_exist()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var endpoint = new Endpoint("name", "sip", "pin", null);
        
        Action action = () => screening.RemoveEndpoint(endpoint);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message == DomainRuleErrorMessages.EndpointDoesNotExistForScreening);
    }
}