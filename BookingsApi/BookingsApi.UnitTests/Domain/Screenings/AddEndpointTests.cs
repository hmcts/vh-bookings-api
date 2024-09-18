using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Screenings;

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