using BookingsApi.Client;

namespace BookingsApi.IntegrationTests.Api.V2.WorkAllocation;

public class GetUnallocatedHearingsTests : ApiTest
{
    [Test]
    public async Task should_return_allocated_hearings()
    {
        // arrange
        var nonGenericCaseType = "Financial Remedy";
        var unallocatedHearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddDays(1);
            options.CaseTypeName = nonGenericCaseType;
        });
        var allocatedHearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.ScheduledDate = DateTime.UtcNow.AddDays(1);
            options.CaseTypeName = nonGenericCaseType;
        });
        var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1");
        await Hooks.AddAllocation(allocatedHearing, justiceUser);
        
        // act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        var unallocatedHearings = await bookingsApiClient.GetUnallocatedHearingsV2Async();
        
        // assert
        unallocatedHearings.Should().NotContain(x => x.Id == allocatedHearing.Id);
        unallocatedHearings.Should().Contain(x => x.Id == unallocatedHearing.Id);
    }
}