using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class GetHearingsForNotificationTest: ApiTest
{
    [Test]
    public async Task should_return_only_first_day_of_multibooking()
    {
        // arrange
        var startingDate = DateTime.UtcNow.AddDays(2);
        var hearing1 = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate;
        });

        var sourceId = hearing1.SourceId;

        var dates = new List<DateTime> {startingDate.AddDays(3), startingDate.AddDays(4)};
        await Hooks.CloneVideoHearing(hearing1.Id, dates);

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingsForNotification());

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var hearingsForNotification = await ApiClientResponse.GetResponses<List<HearingNotificationResponse>>(result.Content);
        hearingsForNotification.Count.Should().Be(1);
        hearingsForNotification[0].Hearing.GroupId.Should().Be(hearing1.Id);
    }
    
}