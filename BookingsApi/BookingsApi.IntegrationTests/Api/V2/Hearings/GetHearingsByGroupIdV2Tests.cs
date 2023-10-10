using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.IntegrationTests.Api.V2.Hearings;

public class GetHearingsByGroupIdV2Tests : ApiTest
{
    [Test]
    public async Task should_return_all_hearings_in_a_group_by_matching_source_id()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddMinutes(5);
        var hearing1 = await Hooks.SeedVideoHearingV2(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });
        var groupId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(2), startingDate.AddDays(3)};
        await Hooks.CloneVideoHearing(hearing1.Id, dates);

        // act
        using var client = Application.CreateClient();

        // assert
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpointsV2.GetHearingsByGroupId(hearing1.Id));

        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var hearingsForGroup = await ApiClientResponse.GetResponses<List<HearingDetailsResponseV2>>(result.Content);
        hearingsForGroup.TrueForAll(x => x.GroupId == groupId).Should().BeTrue();
    }
}