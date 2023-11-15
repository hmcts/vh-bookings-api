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
    
}