using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class CloneHearingTests : ApiTest
{
    [Test]
    public async Task should_return_all_cloned_hearings_for_the_dates()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var groupId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};

        // act
        using var client = Application.CreateClient();
        var request = new CloneHearingRequest { Dates = dates };
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.CloneHearing(hearing1.Id), RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var clonedHearingsList = await ApiClientResponse.GetResponses<List<HearingDetailsResponse>>(result.Content);
        clonedHearingsList.TrueForAll(x => x.GroupId == groupId).Should().BeTrue();
    }
}