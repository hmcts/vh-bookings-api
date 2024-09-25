using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing;

public class AssignScreeningForEndpointTests
{
    [Test]
    public void should_throw_exception_if_endpoint_participant_from_themself()
    {
        var hearing = new VideoHearingBuilder().Build();
        var endpoint = new Endpoint("display name", "defence", "sip", null);
        hearing.AddEndpoint(endpoint);
        
        Action action = () => hearing.AssignScreeningForEndpoint(endpoint, ScreeningType.Specific, [], ["Display Name"]);
        action.Should().Throw<DomainRuleException>()
            .And.ValidationFailures.Should()
            .Contain(x => x.Message.Contains(DomainRuleErrorMessages.EndpointCannotScreenFromThemself));
    }
}