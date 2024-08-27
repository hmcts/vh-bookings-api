using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

public class GetHearingsForTodayTests : ApiTest
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

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingsForToday());

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var hearings = await ApiClientResponse.GetResponses<List<HearingDetailsResponseV2>>(result.Content);
        hearings.Should().NotBeNullOrEmpty();
        hearings.Should().Contain(h => h.Id == hearing.Id);
        
    }
}