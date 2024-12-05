using BookingsApi.Client;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class GetHearingsByTypesTests : ApiTest
{
    [Test]
    public async Task should_return_v2_bookings()
    {
        // arrange
        var caseName = "TestGetByCaseTypeV2";
        var hearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
        {
            options.AddPanelMember = true;
            options.EndpointsToAdd = 1;
            options.Case = new Case(caseName, caseName);
        });
        
        var request = new GetHearingRequest
        {
            CaseNumber = caseName
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.GetHearingsByTypes, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingResponse = await ApiClientResponse.GetResponses<BookingsResponse>(result.Content);
        hearingResponse.Should().NotBeNull();
        hearingResponse.Hearings[0].Hearings
            .Exists(x => x.HearingId == hearing.Id && x.JudgeName == hearing.GetJudge().DisplayName).Should().BeTrue();
    }

    [Test]
    public async Task should_return_validation_problem_if_case_type_does_not_exist()
    {
        // arrange
        var request = new GetHearingRequest
        {
            Types = [9999999]
        };
        var client = GetBookingsApiClient();
        
        // act
        var act = async () => await client.GetHearingsByTypesAsync(request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey("Hearing types");
        errors["Hearing types"].Should().Contain("Invalid value for hearing types");
    }
    
    [Test]
    public async Task should_return_validation_problem_if_hearing_venue_does_not_exist()
    {
        // arrange
        var request = new GetHearingRequest
        {
            VenueIds = [9999999]
        };
        var client = GetBookingsApiClient();
        
        // act
        var act = async () => await client.GetHearingsByTypesAsync(request);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey("Venue ids");
        errors["Venue ids"].Should().Contain("Invalid value for venue ids");
    }
}