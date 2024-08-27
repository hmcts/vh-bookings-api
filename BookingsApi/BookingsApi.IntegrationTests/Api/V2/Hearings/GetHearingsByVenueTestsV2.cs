using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

public class GetHearingsByVenueNameTests : ApiTest
{
    [Test]
    public async Task should_return_all_hearings_by_venue_name()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing = await Hooks.SeedVideoHearingV2(configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var request = new List<string> { hearing.HearingVenue.Name };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingsForTodayByVenue(), RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var hearings = await ApiClientResponse.GetResponses<List<HearingDetailsResponseV2>>(result.Content);
        hearings.Should().NotBeNullOrEmpty();
        hearings.Should().Contain(h => h.Id == hearing.Id);
        
    }

    [Test]
    public async Task should_return_not_found_for_hearings_by_venue_name()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        await Hooks.SeedVideoHearingV2(configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var request = new List<string> { "None existing court name" };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingsForTodayByVenue(), RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound, result.Content.ReadAsStringAsync().Result);
    }
}