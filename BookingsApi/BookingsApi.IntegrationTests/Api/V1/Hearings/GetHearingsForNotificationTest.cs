using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.Mappings.V1;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class GetHearingsForNotificationTest: ApiTest
{
    [Test]
    public async Task should_return_response_when_first_multi_day_is_in_range_of_query()
    {
        // Arrange
        
        // Single day
        var singleDayHearingOutsideRange1 = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddHours(2);
        });
        var singleDayHearing = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddDays(2).AddHours(2);
        });
        var singleDayHearingOutsideRange2 = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddDays(3).AddHours(2);
        });

        // Multi-day
        var multiDayHearingDates = new List<DateTime>
        {
            DateTime.UtcNow.AddDays(2),
            
            // Outside range
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(4)
        };
        var multiDayHearingDay1 = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = multiDayHearingDates[0];
        }, isMultiDayFirstHearing: true);
        var datesToCloneFor = multiDayHearingDates.Skip(1).ToList();
        await Hooks.CloneVideoHearing(multiDayHearingDay1.Id, datesToCloneFor);
        
        // Act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingsForNotification());
        
        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var responses = await ApiClientResponse.GetResponses<List<HearingNotificationResponse>>(result.Content);
        responses.Count.Should().Be(2);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);

        // Multi day hearing day 1
        var multiDayHearingDay1Response = responses.Find(x => x.Hearing.Id == multiDayHearingDay1.Id);
        multiDayHearingDay1Response.Should().NotBeNull();
        multiDayHearingDay1Response.Hearing.Should().BeEquivalentTo(HearingToDetailsResponseMapper.Map(multiDayHearingDay1));
        multiDayHearingDay1Response.TotalDays.Should().Be(multiDayHearingDates.Count);
        multiDayHearingDay1Response.SourceHearing.Should().BeEquivalentTo(HearingToDetailsResponseMapper.Map(multiDayHearingDay1));

        // Single day hearing
        var singleDayHearingResponse = responses.Find(x => x.Hearing.Id == singleDayHearing.Id);
        AssertSingleDayHearingResponse(singleDayHearingResponse, singleDayHearing);
        
        responses.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange1.Id);
        responses.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange2.Id);
    }
    
    [Test]
    public async Task should_return_response_when_subsequent_multi_day_is_in_range_of_query()
    {
        // Arrange
        
        // Single day
        var singleDayHearingOutsideRange1 = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddHours(2);
        });
        var singleDayHearing = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddDays(2).AddHours(2);
        });
        var singleDayHearingOutsideRange2 = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddDays(3).AddHours(2);
        });

        // Multi-day
        var multiDayHearingDates = new List<DateTime>
        {
            DateTime.UtcNow.AddDays(1), // Outside range
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(3) // Outside range
        };
        var multiDayHearingDay1 = await Hooks.SeedVideoHearing(options =>
        {
            options.ScheduledDate = multiDayHearingDates[0];
        }, isMultiDayFirstHearing: true);
        var datesToCloneFor = multiDayHearingDates.Skip(1).ToList();
        await Hooks.CloneVideoHearing(multiDayHearingDay1.Id, datesToCloneFor);
        
        // Act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.HearingsEndpoints.GetHearingsForNotification());
        
        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var responses = await ApiClientResponse.GetResponses<List<HearingNotificationResponse>>(result.Content);
        responses.Count.Should().Be(2);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);

        // Multi day hearing day 2
        var multiDayHearingDay2 = VideoHearings.Get(db).FirstOrDefault(x => x.ScheduledDateTime == multiDayHearingDates[1]);
        var multiDayHearingDay2Response = responses.Find(x => x.Hearing.Id == multiDayHearingDay2.Id);
        multiDayHearingDay2Response.Should().NotBeNull();
        multiDayHearingDay2Response.Hearing.Should().BeEquivalentTo(HearingToDetailsResponseMapper.Map(multiDayHearingDay2));
        multiDayHearingDay2Response.TotalDays.Should().Be(multiDayHearingDates.Count);
        multiDayHearingDay2Response.SourceHearing.Should().BeEquivalentTo(HearingToDetailsResponseMapper.Map(multiDayHearingDay1));

        // Single day hearing
        var singleDayHearingResponse = responses.Find(x => x.Hearing.Id == singleDayHearing.Id);
        AssertSingleDayHearingResponse(singleDayHearingResponse, singleDayHearing);

        responses.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange1.Id);
        responses.Select(x => x.Hearing.Id).Should().NotContain(singleDayHearingOutsideRange2.Id);
    }

    private static void AssertSingleDayHearingResponse(HearingNotificationResponse response, Hearing hearing)
    {
        response.Should().NotBeNull();
        response.Hearing.Should().BeEquivalentTo(HearingToDetailsResponseMapper.Map(hearing));
        response.TotalDays.Should().Be(1);
        response.SourceHearing.Should().BeNull();
    }
}