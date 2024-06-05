using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Endpoints;

public class GetEndPointTests : ApiTest
{
    [Test] 
    public async Task should_return_not_found()
    {
        // arrange
        using var client = Application.CreateClient();
        
        // act
        var result = await client.GetAsync(ApiUriFactory.JVEndPointEndpoints.GetEndpoint("RandomString"));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task should_get_endpoint_by_sip()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(options =>
        {
            options.EndpointsToAdd = 3;
        });
        using var client = Application.CreateClient();
        var endpoint = seededHearing.GetEndpoints()[0];
        // act
        var result = await client.GetAsync(ApiUriFactory.JVEndPointEndpoints.GetEndpoint(endpoint.Sip));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var endpointResponse = await ApiClientResponse.GetResponses<EndpointResponseV2>(result.Content);
        endpointResponse.Id.Should().Be(endpoint.Id);
        endpointResponse.DisplayName.Should().Be(endpoint.DisplayName);
        endpointResponse.Sip.Should().Be(endpoint.Sip);
        endpointResponse.Pin.Should().Be(endpoint.Pin);
        endpointResponse.EndpointParticipants.ForEach(epr => endpoint.EndpointParticipants.Should().Contain(ep => ep.ParticipantId == epr.ParticipantId));
    }
    

}