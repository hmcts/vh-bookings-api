using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Helper;

namespace BookingsApi.IntegrationTests.Api.V1.WorkAllocation;

public class SearchForAllocationHearingsTests: ApiTest
{
    
    [Test]
    public async Task should_return_hearings_that_match_query()
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
        db.Allocations.Add(new Allocation()
        {
            HearingId = hearingWithWorkAllocationVenueAndAllocation.Id,
            JusticeUserId = justiceUser.Id
        });
        await db.SaveChangesAsync();
        
        var hearingWithoutWorkAllocationVenue = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case(caseNumber,"Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
            options.HearingVenue = venueWithoutWorkAllocationEnabled;
        });
        
        using var client = Application.CreateClient();
        
        // act
        var result = await client.GetAsync(ApiUriFactory.WorkAllocationEndpoints.SearchForAllocationHearings(
            new SearchForAllocationHearingsRequest()
            {
                CaseNumber = caseNumber
            }));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingAllocationSearchResponse = await ApiClientResponse.GetResponses<List<HearingAllocationsResponse>>(result.Content);
        hearingAllocationSearchResponse.Should().NotBeEmpty();
        hearingAllocationSearchResponse.Should().Contain(response => response.HearingId == hearingWithWorkAllocationVenue.Id)
            .Subject.AllocatedCso.Should().Be(VideoHearingHelper.NotAllocated);
        
        hearingAllocationSearchResponse.Should().Contain(response => response.HearingId == hearingWithWorkAllocationVenueAndAllocation.Id)
            .Subject.AllocatedCso.Should().Be(justiceUser.Username);

        hearingAllocationSearchResponse.Should()
            .NotContain(response => response.HearingId == hearingWithoutWorkAllocationVenue.Id);
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await Hooks.ClearSeededJusticeUsersAsync();
        await Hooks.ClearSeededHearings();
    }
}