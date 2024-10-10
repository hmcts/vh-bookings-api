using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class RemoveEndpointTests
{
    [Test]
    public void should_remove_endpoint_from_screening()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var endpoint = new Endpoint(Guid.NewGuid().ToString(),"name", "sip", "pin", null);
        
        screening.UpdateScreeningList([], [endpoint]);
        screening.RemoveEndpoint(endpoint);
        
        screening.GetEndpoints().Should().NotContain(x => x.Endpoint == endpoint);
    }
    
    [Test]
    public void should_throw_exception_when_endpoint_does_not_exist()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var endpoint = new Endpoint(Guid.NewGuid().ToString(),"name", "sip", "pin", null);
        
        Action action = () => screening.RemoveEndpoint(endpoint);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message == DomainRuleErrorMessages.EndpointDoesNotExistForScreening);
    }
}