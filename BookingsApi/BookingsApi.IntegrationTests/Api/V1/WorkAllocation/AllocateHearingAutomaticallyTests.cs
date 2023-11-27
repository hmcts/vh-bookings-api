using BookingsApi.Contract.V1.Responses;
using Testing.Common.Assertions;
using DayOfWeek = System.DayOfWeek;

namespace BookingsApi.IntegrationTests.Api.V1.WorkAllocation;

public class AllocateHearingAutomaticallyTests : ApiTest
{
    private List<JusticeUser> _existingActiveUsers;

    [SetUp]
    public async Task Setup()
    {
        await StoreExistingUsers();
    }
    
    [Test]
    public async Task should_return_400_and_error_message_when_no_cso_available_to_allocate_to_a_hearing()
    {
        // arrange
        var caseNumber = "TestSearchQueryInt";
        var nonGenericCaseTypeName = "Financial Remedy";
        var hearing = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case(caseNumber,"Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
        });
        
        // act
        using var client = Application.CreateClient();
        var result =
            await client.PostAsync(ApiUriFactory.WorkAllocationEndpoints.AllocateHearingAutomatically(hearing.Id),
                new StringContent(string.Empty));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.ContainsKeyAndErrorMessage("NoCsosAvailable", $"Unable to allocate to hearing {hearing.Id}, no CSOs available");
    }
    
    [Test]
    public async Task should_return_400_and_error_message_when_no_hearing_found()
    {
        // arrange
        var hearingId = Guid.NewGuid();
        
        // act
        using var client = Application.CreateClient();
        var result =
            await client.PostAsync(ApiUriFactory.WorkAllocationEndpoints.AllocateHearingAutomatically(hearingId), new StringContent(string.Empty));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.ContainsKeyAndErrorMessage("HearingNotFound", $"Hearing {hearingId} not found");
    }

    [Test]
    public async Task should_return_400_and_error_message_when_hearing_already_allocated()
    {
        // arrange
        var caseNumber = "TestSearchQueryInt";
        var nonGenericCaseTypeName = "Financial Remedy";
        var hearing = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case(caseNumber, "Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
        });
        var j1 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1");
        await Hooks.AddAllocation(hearing, j1);
        
        // act
        using var client = Application.CreateClient();
        var result =
            await client.PostAsync(ApiUriFactory.WorkAllocationEndpoints.AllocateHearingAutomatically(hearing.Id), new StringContent(string.Empty));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.ContainsKeyAndErrorMessage("HearingAlreadyAllocated", $"Hearing {hearing.Id} has already been allocated");
    }

    [Test]
    public async Task should_return_200_and_allocated_justice_user_when_allocation_succeeds()
    {
        // arrange
        var caseNumber = "TestSearchQueryInt";
        var nonGenericCaseTypeName = "Financial Remedy";
        var hearing = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case(caseNumber, "Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
            options.ScheduledDate = MoveHearingToWorkingDay(); // Needed as the seeded justice users do not work on weekends
        });

        var j1 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1", initWorkHours:true);
        
        // act
        using var client = Application.CreateClient();
        var result =
            await client.PostAsync(ApiUriFactory.WorkAllocationEndpoints.AllocateHearingAutomatically(hearing.Id), new StringContent(string.Empty));
        
        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var response = await ApiClientResponse.GetResponses<JusticeUserResponse>(result.Content);
        response.Id.Should().Be(j1.Id);
        response.FirstName.Should().Be(j1.FirstName);
        response.Lastname.Should().Be(j1.Lastname);
        response.Username.Should().Be(j1.Username);
    }


    [TearDown]
    public new async Task TearDown()
    {
        await Hooks.ClearSeededHearings();
        await Hooks.ClearSeededJusticeUsersAsync();
        await RestoreExistingJusticeUsers();
    }

    private async Task StoreExistingUsers()
    {
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        _existingActiveUsers = await db.JusticeUsers.IgnoreQueryFilters().Where(x => !x.Deleted).ToListAsync();
        _existingActiveUsers.ForEach(x => x.Delete());
        db.JusticeUsers.UpdateRange(_existingActiveUsers);
        await db.SaveChangesAsync();
    }

    private async Task RestoreExistingJusticeUsers()
    {
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        foreach (var existingUser in _existingActiveUsers)
        {
            var user = await db.JusticeUsers.IgnoreQueryFilters().FirstAsync(x => x.Id == existingUser.Id);
            user.Restore();
        }
        await db.SaveChangesAsync();
    }

    private static DateTime MoveHearingToWorkingDay()
    {
        var scheduledDateTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10).AddMinutes(30);

        if (scheduledDateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            scheduledDateTime = scheduledDateTime.AddDays(2);
        }
        return scheduledDateTime;
    }
}