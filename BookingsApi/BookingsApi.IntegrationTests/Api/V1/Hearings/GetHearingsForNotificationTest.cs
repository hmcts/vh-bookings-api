using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class GetHearingsForNotificationTest: ApiTest
{
    [Test]
    public async Task should_return_only_first_day_of_multibooking_and_singleday_within_the_cutoff()
    {
        // arrange
        
        // Multi day hearing before the cutoff
        var startingDate1 = DateTime.UtcNow.Date.AddHours(6);
        var multiDayHearingBeforeCutOff = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate1;
        });
        
        var dates1 = new List<DateTime> {startingDate1.AddDays(1), startingDate1.AddDays(2)};
        await Hooks.CloneVideoHearing(multiDayHearingBeforeCutOff.Id, dates1);
        
        // Multi day hearing after the cutoff
        var startingDate2 = DateTime.UtcNow.Date.AddDays(4);
        var multiDayHearingAfterCutOff = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate2;
        });
        
        var dates2 = new List<DateTime> {startingDate2.AddDays(5), startingDate2.AddDays(6)};
        await Hooks.CloneVideoHearing(multiDayHearingAfterCutOff.Id, dates2);
        
        // Multi day hearing within the cutoff
        var startingDate3 = DateTime.UtcNow.Date.AddDays(2);
        var multiDayHearingWithinCutOff = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate3;
        });
        
        var dates3 = new List<DateTime> {startingDate3.AddDays(3), startingDate3.AddDays(4)};
        await Hooks.CloneVideoHearing(multiDayHearingWithinCutOff.Id, dates3);
        
        // Single day hearing before the cutoff
        var startingDate4 = DateTime.UtcNow.Date.AddHours(6);
        var singleDayBeforeCutOff = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:false, configureOptions: options =>
        {
            options.ScheduledDate = startingDate4;
        });
        
        // Single day hearing after the cutoff
        var startingDate5 = DateTime.UtcNow.Date.AddDays(4);
        var singleDayAfterCutOff = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:true, configureOptions: options =>
        {
            options.ScheduledDate = startingDate5;
        });
        
        // Single day hearing within the cutoff
        var startingDate6 = DateTime.UtcNow.Date.AddDays(2).AddHours(3);
        var singleDayWithinCutOff = await Hooks.SeedVideoHearing(isMultiDayFirstHearing:false, configureOptions: options =>
        {
            options.ScheduledDate = startingDate6;
        });

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingsForNotification());

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var hearingsForNotification = await ApiClientResponse.GetResponses<List<HearingNotificationResponse>>(result.Content);
        hearingsForNotification.Should().Contain(x=> x.Hearing.Id == singleDayWithinCutOff.Id);
        hearingsForNotification.Find(x=> x.Hearing.Id == singleDayWithinCutOff.Id).TotalDays.Should().Be(1);
        hearingsForNotification.Should().Contain(x=> x.Hearing.Id == multiDayHearingWithinCutOff.Id);
        hearingsForNotification.Find(x=> x.Hearing.Id == multiDayHearingWithinCutOff.Id).TotalDays.Should().Be(3);
        hearingsForNotification.Select(x => x.Hearing.Id).Should().NotContain(new List<Guid>()
        {
            singleDayBeforeCutOff.Id, singleDayAfterCutOff.Id, multiDayHearingBeforeCutOff.Id, multiDayHearingAfterCutOff.Id
        });
    }
    
}