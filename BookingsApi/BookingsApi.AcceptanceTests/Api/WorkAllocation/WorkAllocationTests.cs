using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.AcceptanceTests.Models;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using UpdateBookingStatusRequest = BookingsApi.Contract.Requests.UpdateBookingStatusRequest;

namespace BookingsApi.AcceptanceTests.Api.WorkAllocation;

public class WorkAllocationTests : ApiTest
{
    private HearingDetailsResponse _hearing;
    private JusticeUserResponse _cso;

    [SetUp]
    public async Task Setup()
    {
        var users = await _client.GetJusticeUserListAsync(null);
        users.Should().NotBeNullOrEmpty("We need a CSO to allocate to a hearing");
        _cso = users.First(x =>
            x.FirstName.Contains("test", StringComparison.CurrentCultureIgnoreCase) ||
            x.FirstName.Contains("Auto", StringComparison.CurrentCultureIgnoreCase));
        _cso.Should().NotBeNull("We need a test justice user to modify for testing");
    }
    
    [TearDown]
    public void TearDown()
    {
        if (_hearing == null) return;
        _client.UpdateBookingStatusAsync(_hearing.Id,
            new UpdateBookingStatusRequest
                {UpdatedBy = "AC Test", CancelReason = "Test Over", Status = UpdateBookingStatus.Cancelled});
        _hearing = null;
    }

    [Test]
    public async Task should_assign_a_cso_to_a_hearing()
    {
        // arrange
        var hearingSchedule = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(20);
        var caseName = "Bookings Api AC Automated";
        var bookNewHearingRequest = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
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
        _hearing = await _client.BookNewHearingAsync(bookNewHearingRequest);

        // act
        var updatedHearings = await _client.AllocateHearingsToCsoAsync(new UpdateHearingAllocationToCsoRequest
        {
            Hearings = new List<Guid> {_hearing.Id},
            CsoId = _cso.Id
        });

        // assert
        updatedHearings.Should().HaveCount(1);
        var updatedHearing = updatedHearings[0];
        AssertHearingAllocationResponse(updatedHearing, bookNewHearingRequest);

        var searchResult =
            await _client.SearchForAllocationHearingsAsync(null, null, new List<Guid>(){_cso.Id}, new List<string>(), null,false);
        searchResult.Should().NotBeNullOrEmpty();
        var searchedHearing = searchResult.First(x => x.HearingId == _hearing.Id);
        AssertHearingAllocationResponse(searchedHearing, bookNewHearingRequest);
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
        var failed = await _client.SaveWorkHoursAsync(workHoursRequests);
        failed.Should().BeEmpty("saving VHO Hours should have succeeded");
    }

    private async Task ClearCsoExistingNonAvailabilities(DateTime startTime, DateTime endTime)
    {
        var nonAvailabilitiesResponse = await _client.GetVhoNonAvailabilityHoursAsync(_cso.Username);
        var nonAvailabilities = nonAvailabilitiesResponse.Where(na => na.StartTime <= endTime.Date)
            .Where(na => startTime.Date <= na.EndTime);

        foreach (var na in nonAvailabilities)
        {
            await _client.DeleteVhoNonAvailabilityHoursAsync(na.Id);
        }
    }

}