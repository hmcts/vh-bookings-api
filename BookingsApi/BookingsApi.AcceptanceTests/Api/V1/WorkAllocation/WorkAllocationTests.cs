using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api.V1.Request;

namespace BookingsApi.AcceptanceTests.Api.V1.WorkAllocation;

public class WorkAllocationTests : ApiTest
{
    private HearingDetailsResponse _hearing;
    private JusticeUserResponse _cso;
    private const string CaseName = "Bookings Api AC Automated";
    
    [SetUp]
    public async Task Setup()
    {
        _cso = await GetJusticeUserForTest();
        TestContext.WriteLine($"Using justice user {_cso.Id} - {_cso.Username}");
        _cso.Should().NotBeNull("We need a test justice user to modify for testing");
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_hearing == null) return;
        await BookingsApiClient.RemoveHearingAsync(_hearing.Id);
        TestContext.WriteLine("Removed hearing");
        _hearing = null;
        await BookingsApiClient.DeleteJusticeUserAsync(_cso.Id);
        TestContext.WriteLine($"Deleted justice user {_cso.Id}");
    }

    [Test]
    public async Task should_assign_a_cso_to_a_hearing()
    {
        // arrange
        var hearingSchedule = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(20);
        var bookNewHearingRequest = new SimpleBookNewHearingRequest(CaseName, hearingSchedule).Build();
        // setup CSO to be working during hearing
        var startTime = bookNewHearingRequest.ScheduledDateTime;
        var endTime = bookNewHearingRequest.ScheduledDateTime.AddMinutes(bookNewHearingRequest.ScheduledDuration);
        var csoStart = startTime.AddHours(-1);
        var csoEnd = endTime.AddHours(1);

        // create working hours for every day of the week
        var workingHours = (from object val in Enum.GetValues(typeof(DayOfWeek))
            select (int) val + 1
            into dayOfWeek
            select new WorkingHours(dayOfWeek, csoStart.Hour, csoStart.Minute, csoEnd.Hour, csoEnd.Minute)).ToList();

        var workingHourRequests = new List<UploadWorkHoursRequest>
        {
            new()
            {
                Username = _cso.Username, WorkingHours = workingHours
            }
        };
        
        await UpdateCsoWorkingHoursTo(workingHourRequests);
        await ClearCsoExistingNonAvailabilities(startTime, endTime);
        _hearing = await BookingsApiClient.BookNewHearingAsync(bookNewHearingRequest);

        // act
        var updatedHearings = await BookingsApiClient.AllocateHearingsToCsoAsync(new UpdateHearingAllocationToCsoRequest
        {
            Hearings = new List<Guid> {_hearing.Id},
            CsoId = _cso.Id
        });

        // assert
        updatedHearings.Should().HaveCount(1);
        var updatedHearing = updatedHearings.First();
        AssertHearingAllocationResponse(updatedHearing, bookNewHearingRequest);

        var searchResult =
            await BookingsApiClient.SearchForAllocationHearingsAsync(null, null, new List<Guid>(){_cso.Id}, new List<string>(), null,false);
        searchResult.Should().NotBeNullOrEmpty();
        var searchedHearing = searchResult.First(x => x.HearingId == _hearing.Id);
        AssertHearingAllocationResponse(searchedHearing, bookNewHearingRequest);
    }

    [Test]
    public async Task should_call_getAllocation_with_1000_ids_and_not_fail_and_return_the_valid_hearing()
    {
        // arrange
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        
        var bookNewHearingRequest = new SimpleBookNewHearingRequest(CaseName, hearingSchedule).Build();
        _hearing = await BookingsApiClient.BookNewHearingAsync(bookNewHearingRequest);
        await BookingsApiClient.AllocateHearingsToCsoAsync(new UpdateHearingAllocationToCsoRequest
        {
            Hearings = new List<Guid> {_hearing.Id},
            CsoId = _cso.Id
        });
        var testParameters = new List<Guid>(){_hearing.Id};
        for (int i = 0; i < 1000; i++)
            testParameters.Add(Guid.NewGuid());

        // act
        var results = await BookingsApiClient.GetAllocationsForHearingsAsync(testParameters);
        
        // assert
        results.Count.Should().BeGreaterThan(0);
        results.Should().Contain(e => e.HearingId == _hearing.Id);
    }

    
    [Test]
    public async Task should_call_get_allocation_by_hearing_venue_and_return_valid_hearing()
    {
        // arrange
        var hearingSchedule = DateTime.UtcNow.AddHours(1);
        
        var bookNewHearingRequest = new SimpleBookNewHearingRequest(CaseName, hearingSchedule).Build();
        _hearing = await BookingsApiClient.BookNewHearingAsync(bookNewHearingRequest);
        var allocations = await BookingsApiClient.AllocateHearingsToCsoAsync(new UpdateHearingAllocationToCsoRequest
        {
            Hearings = new List<Guid> {_hearing.Id},
            CsoId = _cso.Id
        });
        
        // act
        var allocation = allocations.First();
        var results = await BookingsApiClient.GetAllocationsForHearingsByVenueAsync(new[]{_hearing.HearingVenueName});
        
        // assert
        var result = results.FirstOrDefault(e => e.HearingId == allocation.HearingId);
        result.Should().NotBeNull();
        result.Cso?.Username.Should().Be(allocation.AllocatedCso);
        
    }

    
    private void AssertHearingAllocationResponse(HearingAllocationsResponse hearing, BookNewHearingRequest bookNewHearingRequest)
    {
        hearing.HearingId.Should().Be(_hearing.Id);
        hearing.AllocatedCso.Should().Be(_cso.Username);
        hearing.CaseNumber.Should().Be(bookNewHearingRequest.Cases[0].Number);
        hearing.CaseType.Should().Be(bookNewHearingRequest.CaseTypeName);
        hearing.ScheduledDateTime.Should().Be(_hearing.ScheduledDateTime);
        hearing.Duration.Should().Be(_hearing.ScheduledDuration);
        hearing.HasWorkHoursClash.Should().Be(false);
    }
    
    private async Task UpdateCsoWorkingHoursTo(List<UploadWorkHoursRequest> workHoursRequests)
    {
        var failed = await BookingsApiClient.SaveWorkHoursAsync(workHoursRequests);
        failed.Should().BeEmpty("saving VHO Hours should have succeeded");
    }

    private async Task ClearCsoExistingNonAvailabilities(DateTime startTime, DateTime endTime)
    {
        var nonAvailabilitiesResponse = await BookingsApiClient.GetVhoNonAvailabilityHoursAsync(_cso.Username);
        var nonAvailabilities = nonAvailabilitiesResponse.Where(na => na.StartTime <= endTime.Date)
            .Where(na => startTime.Date <= na.EndTime);

        foreach (var na in nonAvailabilities)
        {
            await BookingsApiClient.DeleteVhoNonAvailabilityHoursAsync(_cso.Username, na.Id);
        }
    }
    
    private async Task<JusticeUserResponse> GetJusticeUserForTest()
    {
        // get all justice users, including deleted ones
        var users = await BookingsApiClient.GetJusticeUserListAsync("automation", true);
            
        // get the first user that contains the word test or Auto else create one
        var cso = users.FirstOrDefault(x =>
                      x.FirstName.Contains("automation", StringComparison.CurrentCultureIgnoreCase) ||x.FirstName.Contains("Auto", StringComparison.CurrentCultureIgnoreCase)) ??
                  await CreateJusticeUser();

        // if a user was returned and it was deleted, restore it before you use it for a test
        if (cso.Deleted)
        {
            TestContext.WriteLine($"Restoring justice user {cso.Id} - {cso.Username}");
            await BookingsApiClient.RestoreJusticeUserAsync(new RestoreJusticeUserRequest()
            {
                Id = cso.Id, Username = cso.Username
            });
        }
            
        return cso;
    }
}