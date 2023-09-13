using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.Endpoints;

public class UpdateEndpointTests : ApiTest
{
    [Test]
    public async Task should_return_bad_request_when_an_invalid_hearing_id_is_provided()
    {
        // arrange
        var hearingId = Guid.Empty;
        var endpointId = Guid.Empty;
        var request = new UpdateEndpointRequest()
        {
            DisplayName = "Updated Endpoint Test",
            DefenceAdvocateContactEmail = null
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PatchAsync(ApiUriFactory.JVEndPointEndpoints.UpdateEndpoint(hearingId, endpointId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["hearingId"][0].Should().Be($"Please provide a valid hearingId");
    }
    
    [Test]
    public async Task should_return_bad_request_when_an_invalid_payload_is_provided()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var endpointId = Guid.Empty;
        var request = new UpdateEndpointRequest()
        {
            DisplayName = string.Empty,
            DefenceAdvocateContactEmail = null,
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PatchAsync(ApiUriFactory.JVEndPointEndpoints.UpdateEndpoint(hearingId, endpointId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.DisplayName)][0].Should()
            .Be(UpdateEndpointRequestValidation.NoDisplayNameError);
    }
    
    [Test]
    public async Task should_return_not_found_when_updating_an_endpoint_to_a_hearing_that_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var endpointId = Guid.NewGuid();
        var request = new UpdateEndpointRequest()
        {
            DisplayName = "Auto Updated Endpoint",
            DefenceAdvocateContactEmail = null
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PatchAsync(ApiUriFactory.JVEndPointEndpoints.UpdateEndpoint(hearingId, endpointId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Hearing {hearingId} does not exist");
    }
    
    [Test]
    public async Task should_return_not_found_when_updating_an_endpoint_that_does_not_exist()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, BookingStatus.Created);
        var hearingId = seededHearing.Id;
        var endpointId = Guid.NewGuid();
        var request = new UpdateEndpointRequest()
        {
            DisplayName = "Auto Updated Endpoint",
            DefenceAdvocateContactEmail = null
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PatchAsync(ApiUriFactory.JVEndPointEndpoints.UpdateEndpoint(hearingId, endpointId), RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Endpoint {endpointId} does not exist");
    }

    [Test]
    public async Task should_return_no_content_when_endpoint_display_name_and_defence_advocate_is_updated()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing(null, BookingStatus.Created, 1);
        var ep = seededHearing.Endpoints[0];
        var rep = seededHearing.GetParticipants().First(x => x.HearingRole.UserRole.IsRepresentative);
        var hearingId = seededHearing.Id;
        var endpointId = ep.Id;
        var request = new UpdateEndpointRequest()
        {
            DisplayName = "Auto Updated Endpoint",
            DefenceAdvocateContactEmail = rep.Person.ContactEmail
        };
        // act
        using var client = Application.CreateClient();
        var result = await client.PatchAsync(ApiUriFactory.JVEndPointEndpoints.UpdateEndpoint(hearingId, endpointId), RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var hearingFromDb = db.VideoHearings.Include(x => x.Endpoints).ThenInclude(x => x.DefenceAdvocate)
            .ThenInclude(x => x.Person).Include(x => x.Participants).AsNoTracking().First(x => x.Id == hearingId);
        var endpoint = hearingFromDb.Endpoints.First(x=>x.DisplayName == request.DisplayName);
        endpoint.DisplayName.Should().Be(request.DisplayName);
        endpoint.DefenceAdvocate.Should().NotBeNull();
        endpoint.DefenceAdvocate.Person.ContactEmail.Should().Be(request.DefenceAdvocateContactEmail);
    }
}