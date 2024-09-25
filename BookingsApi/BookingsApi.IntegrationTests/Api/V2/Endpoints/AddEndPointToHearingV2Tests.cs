using BookingsApi.Client;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.IntegrationTests.Api.V2.Endpoints;

public class AddEndPointToHearingV2Tests : ApiTest
{
    [Test]
    public async Task should_add_endpoint_to_hearing()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);

        var request = new EndpointRequestV2()
        {
            DisplayName = "add-endpoint"
        };
        
        // act
        var response = await bookingsApiClient.AddEndPointToHearingV2Async(hearing.Id, request);
        
        // assert
        response.Should().NotBeNull();
        response.DisplayName.Should().Be(request.DisplayName);
    }
    
    [Test]
    public async Task should_return_validation_problem_when_adding_endpoint_with_invalid_data()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);

        var request = new EndpointRequestV2();
        
        // act
        Func<Task> act = async () => await bookingsApiClient.AddEndPointToHearingV2Async(hearing.Id, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey(nameof(request.DisplayName));
        errors[nameof(request.DisplayName)].Should().Contain("'Display Name' must not be empty.");
    }

    [Test]
    public async Task should_return_not_found_when_adding_to_a_hearing_that_does_not_exist()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        var request = new EndpointRequestV2()
        {
            DisplayName = "add-endpoint"
        };
        
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        
        
        // act
        Func<Task> act = async () => await bookingsApiClient.AddEndPointToHearingV2Async(hearingId, request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        var bookingsApiException = exception.And.As<BookingsApiException>();
        bookingsApiException.Response.Should().Contain($"Hearing {hearingId} does not exist");
    }
    
    [Test]
    public async Task should_add_endpoint_with_screening()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearingV2(options => { options.Case = new Case("Case1 Num", "Case1 Name"); },
            BookingStatus.Created);

        var individual = hearing.GetParticipants().First(x => x is Individual);
        var endpoint = hearing.Endpoints[0];
        
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);

        var request = new EndpointRequestV2()
        {
            DisplayName = "add-endpoint",
            Screening = new ScreeningRequest()
            {
                Type = ScreeningType.Specific,
                ProtectFromEndpoints = [endpoint.DisplayName],
                ProtectFromParticipants = [individual.Person.ContactEmail]
            }
        };
        
        // act
        var response = await bookingsApiClient.AddEndPointToHearingV2Async(hearing.Id, request);
        
        // assert
        response.Should().NotBeNull();
        response.DisplayName.Should().Be(request.DisplayName);
        response.Screening.Should().NotBeNull();
        response.Screening.Type.Should().Be(request.Screening.Type);
        response.Screening.ProtectFromParticipantsIds.Should().BeEquivalentTo([individual.Id]);
        response.Screening.ProtectFromEndpointsIds.Should().BeEquivalentTo([endpoint.Id]);
    }
}