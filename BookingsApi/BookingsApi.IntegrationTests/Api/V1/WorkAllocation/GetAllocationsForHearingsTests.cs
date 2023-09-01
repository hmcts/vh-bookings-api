using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.WorkAllocation;

public class GetAllocationsForHearingsTests : ApiTest
{
    [SetUp]
    public async Task Setup()
    {
        // ensure justice user from test is removed from any potential failed previous runs
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var user = await db.JusticeUsers.FirstOrDefaultAsync(x => x.Username == "user@test.com");
        if (user != null)
        {
            TestContext.WriteLine("Removing user from previous run due to failed cleanup");
            Hooks.AddJusticeUserForCleanup(user.Id);
            await Hooks.ClearSeededJusticeUsersAsync();
        }
    }
    
    [Test]
    public async Task should_return_allocations_for_given_hearings()
    {
        // arrange
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var venueWithWorkAllocationEnabled = await db.Venues.FirstAsync(x => x.IsWorkAllocationEnabled);
        var venueWithoutWorkAllocationEnabled = await db.Venues.FirstAsync(x => !x.IsWorkAllocationEnabled);
        var caseNumber = "TestSearchQueryInt";
        var nonGenericCaseTypeName = "Financial Remedy";
        var hearingWithWorkAllocationVenue = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case(caseNumber,"Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
            options.HearingVenue = venueWithWorkAllocationEnabled;
        });

        var justiceUser = await Hooks.SeedJusticeUser("user@test.com", "Test", "User");
        var hearingWithWorkAllocationVenueAndAllocation = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case(caseNumber,"Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
            options.HearingVenue = venueWithWorkAllocationEnabled;
        });
        await Hooks.AddAllocation(hearingWithWorkAllocationVenueAndAllocation, justiceUser);

        var hearingWithoutWorkAllocationVenue = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case(caseNumber,"Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
            options.HearingVenue = venueWithoutWorkAllocationEnabled;
        });
        
        using var client = Application.CreateClient();
        
        // act
        var result = await client.PostAsync(ApiUriFactory.WorkAllocationEndpoints.GetAllocationsForHearings,
            RequestBody.Set(new []
            {
                hearingWithWorkAllocationVenue.Id,
                hearingWithWorkAllocationVenueAndAllocation.Id,
                hearingWithoutWorkAllocationVenue.Id
            }));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingAllocationSearchResponse = await ApiClientResponse.GetResponses<List<AllocatedCsoResponse>>(result.Content);
        hearingAllocationSearchResponse.Should().NotBeEmpty();


        hearingAllocationSearchResponse.Should().Contain(response =>
            response.HearingId == hearingWithWorkAllocationVenue.Id && 
            response.Cso == null &&
            response.SupportsWorkAllocation);
        
        hearingAllocationSearchResponse.Should().Contain(response =>
            response.HearingId == hearingWithWorkAllocationVenueAndAllocation.Id && 
            response.Cso.Username == justiceUser.Username &&
            response.SupportsWorkAllocation);
        
        
        hearingAllocationSearchResponse.Should().Contain(response =>
            response.HearingId == hearingWithoutWorkAllocationVenue.Id && 
            response.Cso == null &&
            !response.SupportsWorkAllocation);
    }
}