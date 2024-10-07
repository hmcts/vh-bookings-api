using BookingsApi.Client;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.IntegrationTests.Api.V2.Endpoints;

public class UpdateEndpointV2Tests : ApiTest
{
    [Test]
    public async Task should_return_validation_problem_when_updating_with_invalid_data()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpoint = hearing.Endpoints[0];
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = ""
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        Func<Task> act = async () => await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint.Id, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey(nameof(request.DisplayName));
        errors[nameof(request.DisplayName)].Should().Contain("'Display Name' must not be empty.");
    }

    [Test]
    public async Task should_return_not_found_when_hearing_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var endpointId = Guid.NewGuid();
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = "Update"
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        Func<Task> act = async () => await bookingsApiClient.UpdateEndpointV2Async(hearingId, endpointId, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        var bookingsApiException = exception.And.As<BookingsApiException>();
        bookingsApiException.Response.Should().Contain($"Hearing {hearingId} does not exist");
    }
    
    [Test]
    public async Task should_return_not_found_when_endpoint_does_not_exist()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpointId = Guid.NewGuid();
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = "Update"
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        Func<Task> act = async () => await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpointId, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        var bookingsApiException = exception.And.As<BookingsApiException>();
        bookingsApiException.Response.Should().Contain($"Endpoint {endpointId} does not exist");
    }
    
    [Test]
    public async Task should_update_endpoint_and_remove_screening()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var endpoint = hearing.Endpoints[0];
        var screenFrom = hearing.GetParticipants().First(x=> x is Individual);
        
        var request = new UpdateEndpointRequestV2()
        {
            DisplayName = "Updated",
            Screening = new ScreeningRequest()
            {
                Type = ScreeningType.Specific,
                ProtectedFrom = [screenFrom.ExternalReferenceId]
            }
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        // act
        await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint.Id, request);
        
        var response = await bookingsApiClient.GetHearingDetailsByIdV2Async(hearing.Id);
        response.Should().NotBeNull();
        var updatedEndpoint = response.Endpoints.First(x => x.Id == endpoint.Id);
        updatedEndpoint.DisplayName.Should().Be(request.DisplayName);
        updatedEndpoint.Screening.Should().NotBeNull();
        
        
        // act part 2 (remove screening)
        request.Screening = null;
        await bookingsApiClient.UpdateEndpointV2Async(hearing.Id, endpoint.Id, request);
        
        // assert
        response = await bookingsApiClient.GetHearingDetailsByIdV2Async(hearing.Id);
        response.Should().NotBeNull();
        updatedEndpoint = response.Endpoints.First(x => x.Id == endpoint.Id);
        updatedEndpoint.DisplayName.Should().Be(request.DisplayName);
        updatedEndpoint.Screening.Should().BeNull();
    }
}