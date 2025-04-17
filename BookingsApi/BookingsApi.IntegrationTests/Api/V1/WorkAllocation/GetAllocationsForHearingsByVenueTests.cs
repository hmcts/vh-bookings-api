namespace BookingsApi.IntegrationTests.Api.V1.WorkAllocation;

public class GetAllocationsForHearingsByVenueTests : ApiTest
{
    [Test]
    public async Task should_return_allocations_for_venue()
    {
        // arrange
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var venueWithWorkAllocationEnabled = await db.Venues.FirstAsync(x => x.IsWorkAllocationEnabled);
        var caseNumber = $"TestSearchQuery_{Guid.NewGuid():N}";
        var nonGenericCaseTypeName = "Financial Remedy";

        var justiceUser = await Hooks.SeedJusticeUser("user@test.com", "Test", "User");
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case(caseNumber,"Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
            options.HearingVenue = venueWithWorkAllocationEnabled;
            options.ScheduledDate = DateTime.UtcNow.AddHours(1);
        });
        await Hooks.AddAllocation(hearing, justiceUser);
        
        var client = GetBookingsApiClient();
        
        // act

        var result = await client.GetAllocationsForHearingsByVenueAsync([venueWithWorkAllocationEnabled.Name]);
        
        // assert
        var responseHearing = result.First(x=> x.HearingId == hearing.Id);
        responseHearing.Should().NotBeNull();
        responseHearing.Cso.Id.Should().Be(justiceUser.Id);
        responseHearing.SupportsWorkAllocation.Should().BeTrue();

    }
}