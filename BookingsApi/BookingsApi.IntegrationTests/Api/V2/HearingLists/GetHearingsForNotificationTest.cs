using BookingsApi.Client;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.Mappings.V2;

namespace BookingsApi.IntegrationTests.Api.V2.HearingLists;

public class GetHearingsForNotificationTest: ApiTest
{
    [Test]
    public async Task should_return_response_when_first_multi_day_is_in_range_of_query()
    {
        // Arrange
        
        // Single day
        var singleDayHearingOutsideRange1 = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.Date.AddHours(2);
        });
        var singleDayHearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.Date.AddDays(2).AddHours(2);
        });
        var singleDayHearingOutsideRange2 = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.Date.AddDays(3).AddHours(2);
        });

        // Multi-day
        var multiDayHearingDates = new List<DateTime>
        {
            DateTime.UtcNow.Date.AddDays(2),
            
            // Outside range
            DateTime.UtcNow.Date.AddDays(3),
            DateTime.UtcNow.Date.AddDays(4)
        };
        var multiDayHearingDay1 = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = multiDayHearingDates[0];
        }, isMultiDayFirstHearing: true);
        var datesToCloneFor = multiDayHearingDates.Skip(1).ToList();
        await Hooks.CloneVideoHearing(multiDayHearingDay1.Id, datesToCloneFor, duration: 45);
        
        // Act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        var result = await bookingsApiClient.GetHearingsForNotificationAsync();
        
        // Assert
        result.Count.Should().Be(2);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);

        // Multi day hearing day 1
        var multiDayHearingDay1Response = result.First(x => x.Hearing.Id == multiDayHearingDay1.Id);
        multiDayHearingDay1Response.Should().NotBeNull();
        multiDayHearingDay1Response.Hearing.Should().BeEquivalentTo(HearingToDetailsResponseV2Mapper.Map(multiDayHearingDay1));
        multiDayHearingDay1Response.TotalDays.Should().Be(multiDayHearingDates.Count);
        multiDayHearingDay1Response.SourceHearing.Should().BeEquivalentTo(HearingToDetailsResponseV2Mapper.Map(multiDayHearingDay1));

        // Single day hearing
        var singleDayHearingResponse = result.First(x => x.Hearing.Id == singleDayHearing.Id);
        AssertSingleDayHearingResponse(singleDayHearingResponse, singleDayHearing);
        
        result.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange1.Id);
        result.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange2.Id);
    }
    
    [Test]
    public async Task should_return_response_when_subsequent_multi_day_is_in_range_of_query()
    {
        // Arrange
        
        // Single day
        var singleDayHearingOutsideRange1 = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.Date.AddHours(2);
        });
        var singleDayHearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.Date.AddDays(2).AddHours(2);
        });
        var singleDayHearingOutsideRange2 = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.Date.AddDays(3).AddHours(2);
        });

        // Multi-day
        var multiDayHearingDates = new List<DateTime>
        {
            DateTime.UtcNow.Date.AddDays(1), // Outside range
            DateTime.UtcNow.Date.AddDays(2),
            DateTime.UtcNow.Date.AddDays(3) // Outside range
        };
        var multiDayHearingDay1 = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = multiDayHearingDates[0];
        }, isMultiDayFirstHearing: true);
        var datesToCloneFor = multiDayHearingDates.Skip(1).ToList();
        await Hooks.CloneVideoHearing(multiDayHearingDay1.Id, datesToCloneFor, duration: 45);
        
        // Act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        var result = await bookingsApiClient.GetHearingsForNotificationAsync();
        
        // Assert
        result.Count.Should().Be(2);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);

        // Multi day hearing day 2
        var multiDayHearingDay2 = await VideoHearings.Get(db).FirstAsync(x => x.ScheduledDateTime == multiDayHearingDates[1]);
        var multiDayHearingDay2Response = result.First(x => x.Hearing.Id == multiDayHearingDay2.Id);
        multiDayHearingDay2Response.Should().NotBeNull();
        multiDayHearingDay2Response.Hearing.Should().BeEquivalentTo(HearingToDetailsResponseV2Mapper.Map(multiDayHearingDay2));
        multiDayHearingDay2Response.TotalDays.Should().Be(multiDayHearingDates.Count);
        multiDayHearingDay2Response.SourceHearing.Should().BeEquivalentTo(HearingToDetailsResponseV2Mapper.Map(multiDayHearingDay1));

        // Single day hearing
        var singleDayHearingResponse = result.First(x => x.Hearing.Id == singleDayHearing.Id);
        AssertSingleDayHearingResponse(singleDayHearingResponse, singleDayHearing);

        result.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange1.Id);
        result.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange2.Id);
    }

    private static void AssertSingleDayHearingResponse(HearingNotificationResponseV2 response, Hearing hearing)
    {
        response.Should().NotBeNull();
        response.Hearing.Should().BeEquivalentTo(HearingToDetailsResponseV2Mapper.Map(hearing));
        response.TotalDays.Should().Be(1);
        response.SourceHearing.Should().BeNull();
    }
}