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
    public async Task should_return_bad_request_when_hearing_types_are_invalid()
    {
        // arrange
        var request = new GetHearingRequest
        {
            Types = new List<int>{ -1 }
        };
            
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.GetHearingsByTypes, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain($"Invalid value for hearing types");
    }
    
    [Test]
    public async Task should_return_bad_request_when_venue_ids_are_invalid()
    {
        // arrange
        var request = new GetHearingRequest
        {
            VenueIds = new List<int>{ -1 }
        };
            
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.GetHearingsByTypes, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain($"Invalid value for venue ids");
    }
}