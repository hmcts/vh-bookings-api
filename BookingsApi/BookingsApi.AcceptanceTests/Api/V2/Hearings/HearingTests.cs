using System;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Responses;
using FluentAssertions;
using Testing.Common.Builders.Api.V2;

namespace BookingsApi.AcceptanceTests.Api.V2.Hearings;

public class HearingTests : ApiTest
{
    private HearingDetailsResponseV2 _hearing;
    
    [TearDown]
    public async Task TearDown()
    {
        if (_hearing == null) return;
        await BookingsApiClient.RemoveHearingAsync(_hearing.Id);
        TestContext.WriteLine("Removed hearing");
    }

    [Test]
    public async Task should_get_hearings_for_today()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api AC Automated - Get Today";
        var request = new SimpleBookNewHearingRequestV2(caseName, hearingSchedule, SimpleBookNewHearingRequestV2.JudgePersonalCode).Build();
        _hearing = await BookingsApiClient.BookNewHearingWithCodeAsync(request);
        var results = await BookingsApiClient.GetHearingsForTodayV2Async();
        results.Should().NotBeNullOrEmpty();
        results.Should().Contain(e => e.Id == _hearing.Id);
        
    }
    
    [Test]
    public async Task should_get_hearings_for_today_by_venue()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api AC Automated - Get Today By Venue";
        var request = new SimpleBookNewHearingRequestV2(caseName, hearingSchedule, SimpleBookNewHearingRequestV2.JudgePersonalCode).Build();
        _hearing = await BookingsApiClient.BookNewHearingWithCodeAsync(request);
        var results = await BookingsApiClient.GetHearingsForTodayByVenueV2Async([_hearing.HearingVenueName]);
        results.Should().NotBeNullOrEmpty();
        results.Should().Contain(e => e.Id == _hearing.Id);
        foreach (var result in results)
            result.HearingVenueName.Should().Be(_hearing.HearingVenueName, "We should only get hearings for the venue we requested");
        
    }
}