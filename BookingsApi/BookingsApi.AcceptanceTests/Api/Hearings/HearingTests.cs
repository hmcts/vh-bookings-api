using System;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api.Request;

namespace BookingsApi.AcceptanceTests.Api.Hearings;

public class HearingTests : ApiTest
{
    
    private HearingDetailsResponse _hearing;
    
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
        var hearingSchedule = DateTime.UtcNow;
        var caseName = "Bookings Api AC Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        _hearing = await BookingsApiClient.BookNewHearingAsync(request);
        var results = await BookingsApiClient.GetHearingsForTodayAsync();
        results.Should().NotBeNullOrEmpty();
        results.Should().Contain(e => e.Id == _hearing.Id);
        
    }
    
    [Test]
    public async Task should_get_hearings_for_today_by_venue()
    {
        var hearingSchedule = DateTime.UtcNow;
        var caseName = "Bookings Api AC Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        _hearing = await BookingsApiClient.BookNewHearingAsync(request);
        var results = await BookingsApiClient.GetHearingsForTodayByVenueAsync(new []{ _hearing.HearingVenueName });
        results.Should().NotBeNullOrEmpty();
        results.Should().Contain(e => e.Id == _hearing.Id);
        foreach (var result in results)
            result.HearingVenueName.Should().Be(_hearing.HearingVenueName, "We should only get hearings for the venue we requested");
        
    }
}