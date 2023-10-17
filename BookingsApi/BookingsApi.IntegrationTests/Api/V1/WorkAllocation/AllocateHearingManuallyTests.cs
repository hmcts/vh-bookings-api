using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using DayOfWeek = System.DayOfWeek;

namespace BookingsApi.IntegrationTests.Api.V1.WorkAllocation;

public class AllocateHearingManuallyTests : ApiTest
{
    private List<JusticeUser> _existingActiveUsers;

    [SetUp]
    public async Task Setup()
    {
        await StoreExistingUsers();
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
            options.ScheduledDate =
                MoveHearingToWorkingDay(); // Needed as the seeded justice users do not work on weekends
        });
        var judge = hearing.Participants.First(x => x is Judge);

        var j1 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1",
            initWorkHours: true);

        var request = new UpdateHearingAllocationToCsoRequest()
        {
            CsoId = j1.Id,
            Hearings = new List<Guid>() {hearing.Id}
        };

        // act
        using var client = Application.CreateClient();
        var result =
            await client.PatchAsync(ApiUriFactory.WorkAllocationEndpoints.AllocateHearingManually,
                RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var response = await ApiClientResponse.GetResponses<List<HearingAllocationsResponse>>(result.Content);
        response[0].AllocatedCso.Should().Be(j1.Username);
        response[0].HearingId.Should().Be(hearing.Id);

        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeOfType<AllocationHearingsIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as AllocationHearingsIntegrationEvent;
        integrationEvent!.AllocatedCso.Username.Should().Be(j1.Username);
        integrationEvent!.Hearings[0].JudgeDisplayName.Should().Be(judge.DisplayName);
    }

    [Test]
    public async Task should_return_200_and_allocated_justice_user_when_allocation_succeeds_and_judge_is_judiciary()
    {
        // arrange
        var caseNumber = "TestSearchQueryInt";
        var nonGenericCaseTypeName = "Financial Remedy";
        var hearing = await Hooks.SeedVideoHearingV2(options =>
        {
            options.Case = new Case(caseNumber, "Integration");
            options.CaseTypeName = nonGenericCaseTypeName;
            options.ScheduledDate =
                MoveHearingToWorkingDay(); // Needed as the seeded justice users do not work on weekends
        });

        var judge = hearing.JudiciaryParticipants.First(x =>
            x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);

        var j1 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1",
            initWorkHours: true);

        var request = new UpdateHearingAllocationToCsoRequest()
        {
            CsoId = j1.Id,
            Hearings = new List<Guid>() {hearing.Id}
        };

        // act
        using var client = Application.CreateClient();
        var result =
            await client.PatchAsync(ApiUriFactory.WorkAllocationEndpoints.AllocateHearingManually,
                RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK, result.Content.ReadAsStringAsync().Result);
        var response = await ApiClientResponse.GetResponses<List<HearingAllocationsResponse>>(result.Content);
        response[0].AllocatedCso.Should().Be(j1.Username);
        response[0].HearingId.Should().Be(hearing.Id);

        var serviceBusStub =
            Application.Services.GetService(typeof(IServiceBusQueueClient)) as ServiceBusQueueClientFake;
        var message = serviceBusStub!.ReadMessageFromQueue();
        message.IntegrationEvent.Should().BeOfType<AllocationHearingsIntegrationEvent>();
        var integrationEvent = message.IntegrationEvent as AllocationHearingsIntegrationEvent;
        integrationEvent!.AllocatedCso.Username.Should().Be(j1.Username);
        integrationEvent!.Hearings[0].JudgeDisplayName.Should().Be(judge.DisplayName);
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
        var scheduledDateTime = DateTime.UtcNow.Date.AddHours(10).AddMinutes(30);

        if (scheduledDateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            scheduledDateTime = scheduledDateTime.AddDays(2);
        }

        return scheduledDateTime;
    }
}