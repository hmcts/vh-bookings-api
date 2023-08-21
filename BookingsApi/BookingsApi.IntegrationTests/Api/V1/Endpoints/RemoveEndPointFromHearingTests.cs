using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.IntegrationTests.Api.V1.Endpoints;

public class RemoveEndPointFromHearingTests : ApiTest
{
    private ServiceBusQueueClientFake _serviceBusStub;

    [SetUp]
    public void Setup()
    {
        _serviceBusStub = Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
    }

    [Test] public async Task should_return_bad_request_when_an_invalid_hearing_id_is_provided()
    {
        // arrange
        var hearingId = Guid.Empty;
        var endpointId = Guid.NewGuid();
        using var client = Application.CreateClient();
        
        // act
        var result = await client.DeleteAsync(ApiUriFactory.JVEndPointEndpoints.RemoveEndPointFromHearing(hearingId, endpointId));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["hearingId"][0].Should().Be($"Please provide a valid hearingId");
    }
    
    [Test]
    public async Task should_remove_endpoint_from_and_publish_message_when_hearing_is_created_and_found()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, false, BookingStatus.Created, 3);
        using var client = Application.CreateClient();
        var endpoint = seededHearing.GetEndpoints()[0];
        var hearingId = seededHearing.Id;
        var endpointId = endpoint.Id;
        // act
        var result = await client.DeleteAsync(ApiUriFactory.JVEndPointEndpoints.RemoveEndPointFromHearing(hearingId, endpointId));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = await db.VideoHearings.Include(x => x.Endpoints).AsNoTracking()
            .FirstAsync(x => x.Id == hearingId);
        hearingFromDb.GetEndpoints().Any(ep => ep.Id == endpointId).Should().BeFalse();
        var message = _serviceBusStub.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeEquivalentTo(new EndpointRemovedIntegrationEvent(hearingId, endpoint.Sip));
    }
    
    [Test]
    public async Task should_remove_endpoint_from_hearing_but_not_publish_when_hearing_is_not_created()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, false, BookingStatus.Booked, 3);
        using var client = Application.CreateClient();
        var endpoint = seededHearing.GetEndpoints()[0];
        var hearingId = seededHearing.Id;
        var endpointId = endpoint.Id;
        // act
        var result = await client.DeleteAsync(ApiUriFactory.JVEndPointEndpoints.RemoveEndPointFromHearing(hearingId, endpointId));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = await db.VideoHearings.Include(x => x.Endpoints).AsNoTracking()
            .FirstAsync(x => x.Id == hearingId);
        hearingFromDb.GetEndpoints().Any(ep => ep.Id == endpointId).Should().BeFalse();
        var message = _serviceBusStub.ReadMessageFromQueue();
        message.Should().BeNull();
    }

    [Test]
    public async Task should_return_not_found_when_removing_an_endpoint_for_a_hearing_that_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var endpointId = Guid.NewGuid();
        using var client = Application.CreateClient();
        
        // act
        var result = await client.DeleteAsync(ApiUriFactory.JVEndPointEndpoints.RemoveEndPointFromHearing(hearingId, endpointId));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Hearing {hearingId} does not exist");
    }
    
    [Test]
    public async Task should_return_not_found_when_removing_an_endpoint_that_does_not_exist()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, false, BookingStatus.Created, 3);
        using var client = Application.CreateClient();
        var hearingId = seededHearing.Id;
        var endpointId = Guid.NewGuid();
        
        // act
        var result = await client.DeleteAsync(ApiUriFactory.JVEndPointEndpoints.RemoveEndPointFromHearing(hearingId, endpointId));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Endpoint {endpointId} does not exist");
    }
}