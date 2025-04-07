using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.UnitTests.Domain.Screenings;

public class RemoveEndpointTests
{
    [Test]
    public void should_remove_endpoint_from_screening()
    {
        var participant = new ParticipantBuilder().IndividualParticipantApplicant;
        var screening = new Screening(ScreeningType.Specific, participant);
        var endpoint = new Endpoint(Guid.NewGuid().ToString(),"name", "sip", "pin");
        
        screening.UpdateScreeningList([], [endpoint]);
        screening.RemoveEndpoint(endpoint);
        
        screening.GetEndpoints().Should().NotContain(x => x.Endpoint == endpoint);
    }
}