using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using NuGet.Packaging;
using NUnit.Framework;
using DayOfWeek = System.DayOfWeek;

namespace BookingsApi.IntegrationTests.Database.Queries;

public class GetAllocationHearingsBySearchQueryTests : DatabaseTestsBase
{
    private GetAllocationHearingsBySearchQueryHandler _handler;
    private VideoHearing _seededHearing1;
    private VideoHearing _seededHearing2;
    private VideoHearing _seededHearing3;
    private BookingsDbContext _context;

    private const string TestCaseType = "Financial Remedy";
    private readonly DateTime _testDate1 = DateTime.Today.AddDays(50);
    private readonly DateTime _testDate2 = DateTime.Today.AddDays(5);
    
    [SetUp]
    public async Task Setup()
    {
        _context = new BookingsDbContext(BookingsDbContextOptions);
        _handler = new GetAllocationHearingsBySearchQueryHandler(_context, isTest: true);
        _seededHearing1 = await Hooks.SeedVideoHearing(status: BookingStatus.Created, configureOptions: options =>
        {
            options.CaseTypeName = TestCaseType;
            options.ScheduledDate = _testDate1;
        });
        _seededHearing2 = await Hooks.SeedVideoHearing(status: BookingStatus.Booked, configureOptions: options =>
        {
            options.CaseTypeName = TestCaseType;
            options.ScheduledDate = _testDate2;
        });
        _seededHearing3 = await Hooks.SeedVideoHearing(status: BookingStatus.Booked, configureOptions: options =>
        {
            options.ScheduledDate = _testDate1;
        });

    }
    
    [Test]
    public async Task Should_get_hearing_details_by_case_type()
    {
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(caseType: new[]{TestCaseType}));
        //ASSERT
        hearings.Count.Should().Be(2);
        foreach (var hearing in hearings)
            hearing.CaseType.Name.Should().Be(TestCaseType);
    }
    
    [Test]
    public async Task Should_get_hearing_details_by_case_number()
    {
        //ARRANGE
        var hearing = _seededHearing1.HearingCases.First();
        //AAT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(caseNumber:hearing.Case.Number));
        //ASSERT
        hearings.Count.Should().Be(1);
        hearings.First().HearingCases.First().Case.Number.Should().Be(hearing.Case.Number);
    }
    
    [Test]
    public async Task Should_get_hearing_details_by_allocated_cso()
    {
        //ARRANGE
        var justiceUser = await Hooks.SeedJusticeUser(userName: "testUser", null, null, isTeamLead:true);
        await Hooks.AddAllocation(_seededHearing3, justiceUser);
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(cso:new[] {justiceUser.Id}));
        //ASSERT
        hearings.Count.Should().Be(1);
        hearings.First().HearingCases.First().Case.Number.Should().Be(_seededHearing3.HearingCases.First().Case.Number);
        hearings.First().AllocatedTo.Username.Should().Be(justiceUser.Username);
    }   
    
    [Test]
    public async Task Should_get_hearing_details_by_single_date()
    {
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(fromDate:_testDate1));
        //ASSERT
        hearings.Count.Should().Be(2);
        hearings[0].ScheduledDateTime.Should().Be(_seededHearing1.ScheduledDateTime);
        hearings[1].ScheduledDateTime.Should().Be(_seededHearing3.ScheduledDateTime);
    }    
    
    [Test]
    public async Task Should_get_hearing_details_by_date_range()
    {
        //ACT
        var fromDate = DateTime.Today.AddDays(3); //before _testDate2
        var toDate = DateTime.Today.AddDays(7);  //after _testDate2
        
        //ASSERT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(fromDate:fromDate, toDate:toDate));

        //ARRANGE
        hearings.Count.Should().Be(1);
        hearings.First().HearingCases.First().Case.Number.Should().Be(_seededHearing2.HearingCases.First().Case.Number);
        hearings.First().ScheduledDateTime.Should().Be(_seededHearing2.ScheduledDateTime);
    }
    
    [Test]
    public async Task Should_get_hearing_details_by_unallocated()
    {
        //ARRANGE
        await Hooks.AddAllocation(_seededHearing2);
        
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(isUnallocated:true));

        //ASSERT
        hearings.Count.Should().Be(2);
        hearings.Should().Contain(e =>
            e.HearingCases.First().Case.Number == _seededHearing1.HearingCases.First().Case.Number);
        hearings.Should().Contain(e => 
            e.HearingCases.First().Case.Number == _seededHearing3.HearingCases.First().Case.Number);
    }
    
    [Test]
     public async Task Should_include_work_hours_when_requested()
     {
         //ARRANGE
         var justiceUser = await Hooks.SeedJusticeUser(userName: "testUser", null, null, isTeamLead: true);
         var nonavailability = new VhoNonAvailability
             {StartTime = DateTime.Today.AddHours(10), EndTime = DateTime.Today.AddHours(10)};
         justiceUser.VhoNonAvailability.Add(nonavailability);
         var workHours = new List<VhoWorkHours>
         {
             new()
             {
                 StartTime = DateTime.Today.AddHours(10).TimeOfDay, EndTime = DateTime.Today.AddHours(10).TimeOfDay,
                 DayOfWeekId = 1
             },
             new()
             {
                 StartTime = DateTime.Today.AddHours(10).TimeOfDay, EndTime = DateTime.Today.AddHours(10).TimeOfDay,
                 DayOfWeekId = 2
             },
             new()
             {
                 StartTime = DateTime.Today.AddHours(10).TimeOfDay, EndTime = DateTime.Today.AddHours(10).TimeOfDay,
                 DayOfWeekId = 3
             },
             new()
             {
                 StartTime = DateTime.Today.AddHours(10).TimeOfDay, EndTime = DateTime.Today.AddHours(10).TimeOfDay,
                 DayOfWeekId = 4
             },
             new()
             {
                 StartTime = DateTime.Today.AddHours(10).TimeOfDay, EndTime = DateTime.Today.AddHours(10).TimeOfDay,
                 DayOfWeekId = 5
             }
         };
         justiceUser.VhoWorkHours.AddRange(workHours);
         _context.Update(justiceUser);
         await _context.SaveChangesAsync();

         await Hooks.AddAllocation(_seededHearing3, justiceUser);
         //ACT
         var hearings =
             await _handler.Handle(
                 new GetAllocationHearingsBySearchQuery(cso: new[] {justiceUser.Id}, includeWorkHours: true));
         //ASSERT
         hearings.Count.Should().Be(1);
         var allocatedCso = hearings.First().AllocatedTo;

         allocatedCso.Username.Should().Be(justiceUser.Username);
         allocatedCso.VhoWorkHours.Should().NotBeNullOrEmpty();

         allocatedCso.VhoNonAvailability.Should().NotBeNullOrEmpty();
         var nonAvailabilityResult = allocatedCso.VhoNonAvailability[0];
         nonAvailabilityResult.StartTime.Should().Be(nonavailability.StartTime);
         nonAvailabilityResult.EndTime.Should().Be(nonavailability.EndTime);

         allocatedCso.VhoWorkHours[0].StartTime.Should().Be(workHours[0].StartTime);
         allocatedCso.VhoWorkHours[0].EndTime.Should().Be(workHours[0].EndTime);
         allocatedCso.VhoWorkHours[0].SystemDayOfWeek.Should().Be((DayOfWeek) workHours[0].DayOfWeekId);
     }
    
    [Test]
    public async Task Should_get_hearing_details_by_multiple_parameters()
    {
        //ARRANGE
        // hearing 1,2 & 3
        var justiceUser = await Hooks.SeedJusticeUser(userName: "testUser", null, null, isTeamLead:true);
        await Hooks.AddAllocation(_seededHearing1, justiceUser);
        await Hooks.AddAllocation(_seededHearing2, justiceUser);
        await Hooks.AddAllocation(_seededHearing3, justiceUser);
        
        //hearing 1 & 2
        var caseType = TestCaseType;
        
        // hearing 1 & 3
        var fromDate = DateTime.Today.AddDays(48); //before _testDate1
        var toDate = DateTime.Today.AddDays(52);  //after _testDate1

        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(fromDate:fromDate, toDate:toDate, caseType:new[]{caseType}, cso:new[]{justiceUser.Id}));

        //ASSERT
        hearings.Count.Should().Be(1);
        hearings.First().HearingCases.First().Case.Number.Should().Be(_seededHearing1.HearingCases.First().Case.Number);
        hearings.First().ScheduledDateTime.Should().Be(_seededHearing1.ScheduledDateTime);
        hearings.First().CaseType.Name.Should().Be(_seededHearing1.CaseType.Name);
        hearings.First().AllocatedTo.Username.Should().Be(justiceUser.Username);
    }

    [Test]
    public async Task Should_exclude_deleted_work_hours()
    {
        // Arrange
        var justiceUser = await Hooks.SeedJusticeUser(userName: "testUser", null, null, isTeamLead:true);
        await Hooks.AddAllocation(_seededHearing1, justiceUser);

        var deletedWorkHours = new List<VhoWorkHours>();
        
        for (var i = 1; i <= 7; i++)
        {
            deletedWorkHours.Add(new VhoWorkHours
            {
                DayOfWeekId = i, 
                StartTime = new TimeSpan(10, 0, 0), 
                EndTime = new TimeSpan(18, 0, 0),
                JusticeUserId = justiceUser.Id
            });
        }
        
        _context.VhoWorkHours.AddRange(deletedWorkHours);

        await _context.SaveChangesAsync();
        
        justiceUser = _context.JusticeUsers.FirstOrDefault(x => x.Id == justiceUser.Id);
        
        justiceUser.Delete();
        
        await _context.SaveChangesAsync();
        
        justiceUser.Restore();
        
        var nonDeletedWorkHours = new List<VhoWorkHours>();
        
        for (var i = 1; i <= 7; i++)
        {
            nonDeletedWorkHours.Add(new VhoWorkHours
            {
                DayOfWeekId = i, 
                StartTime = new TimeSpan(8, 0, 0), 
                EndTime = new TimeSpan(17, 0, 0),
                JusticeUserId = justiceUser.Id
            });
        }
        
        _context.VhoWorkHours.AddRange(nonDeletedWorkHours);

        await _context.SaveChangesAsync();

        var csos = new List<Guid>() { justiceUser.Id };
        
        // Act
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(includeWorkHours: true, cso: csos));
        
        // Assert
        hearings.Count.Should().Be(1);
        hearings.First().AllocatedTo.Id.Should().Be(justiceUser.Id);
        var resultingWorkHours = hearings.First().Allocations.First().JusticeUser.VhoWorkHours;
        hearings.First().Allocations.First().JusticeUser.VhoWorkHours.Count.Should().Be(nonDeletedWorkHours.Count);
        CollectionAssert.AreEquivalent(resultingWorkHours.Select(wh => wh.Id), nonDeletedWorkHours.Select(wh => wh.Id));
    }
}
