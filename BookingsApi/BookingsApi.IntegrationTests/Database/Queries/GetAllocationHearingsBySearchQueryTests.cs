using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using NuGet.Packaging;
using DayOfWeek = System.DayOfWeek;

namespace BookingsApi.IntegrationTests.Database.Queries;

public class GetAllocationHearingsBySearchQueryTests : DatabaseTestsBase
{
    private GetAllocationHearingsBySearchQueryHandler _handler;
    private VideoHearing _seededHearing1;
    private VideoHearing _seededHearing2;
    private VideoHearing _seededHearing3;
    private VideoHearing _seededHearing4;
    private VideoHearing _seededHearing5;
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
        hearings.TrueForAll(x => x.CaseType.Name == TestCaseType).Should().BeTrue();
    }
    
    [Test]
    public async Task Should_get_hearing_details_by_case_number()
    {
        //ARRANGE
        var hearing = _seededHearing1.HearingCases[0];
        //AAT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(caseNumber:hearing.Case.Number));
        //ASSERT
        hearings.Count.Should().Be(3);
        hearings[0].HearingCases[0].Case.Number.Should().Be(hearing.Case.Number);
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
        hearings[0].HearingCases[0].Case.Number.Should().Be(_seededHearing3.HearingCases[0].Case.Number);
        hearings[0].AllocatedTo.Username.Should().Be(justiceUser.Username);
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
        hearings[0].HearingCases[0].Case.Number.Should().Be(_seededHearing2.HearingCases[0].Case.Number);
        hearings[0].ScheduledDateTime.Should().Be(_seededHearing2.ScheduledDateTime);
    }
    
    [Test]
    public async Task Should_get_hearing_details_by_unallocated()
    {
        //ARRANGE
        await Hooks.AddAllocation(_seededHearing2, null); // null cso will create one
        
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(isUnallocated:true));

        //ASSERT
        hearings.Should().Contain(e =>
            e.HearingCases[0].Case.Number == _seededHearing1.HearingCases[0].Case.Number);
        hearings.Should().Contain(e => 
            e.HearingCases[0].Case.Number == _seededHearing3.HearingCases[0].Case.Number);

        hearings.Should().NotContain(e => e.Id == _seededHearing2.Id);
    }
    
    [Test]
     public async Task Should_include_work_hours_when_requested()
     {
         //ARRANGE
         var justiceUser =
             await Hooks.SeedJusticeUser(userName: "testUser", null, null, isTeamLead: true, initWorkHours: false);
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
         var allocatedCso = hearings[0].AllocatedTo;

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
        hearings[0].HearingCases[0].Case.Number.Should().Be(_seededHearing1.HearingCases[0].Case.Number);
        hearings[0].ScheduledDateTime.Should().Be(_seededHearing1.ScheduledDateTime);
        hearings[0].CaseType.Name.Should().Be(_seededHearing1.CaseType.Name);
        hearings[0].AllocatedTo.Username.Should().Be(justiceUser.Username);
    }

    [Test]
    public async Task Should_exclude_deleted_work_hours()
    {
        // Arrange
        var daysOfWeek = await _context.DaysOfWeek.ToListAsync();
        var justiceUser =
            await Hooks.SeedJusticeUser(userName: "testUser", null, null, isTeamLead: true, initWorkHours: false);
        await Hooks.AddAllocation(_seededHearing1, justiceUser);
        
        for (var i = 1; i <= 7; i++)
        {
            var dayOfWeek = daysOfWeek.First(x => x.Id == i);
            justiceUser.AddOrUpdateWorkHour(dayOfWeek, new TimeSpan(10, 0, 0), new TimeSpan(18, 0, 0));
        }

        await _context.SaveChangesAsync();
        
        justiceUser = _context.JusticeUsers.Include(ju => ju.VhoWorkHours).First(x => x.Id == justiceUser.Id);
        
        justiceUser.Delete();
        
        await _context.SaveChangesAsync();
        
        justiceUser.Restore();
        
        for (var i = 1; i <= 7; i++)
        {
            var dayOfWeek = daysOfWeek.First(x => x.Id == i);
            justiceUser.AddOrUpdateWorkHour(dayOfWeek, new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0));
        }

        var nonDeletedWorkHours = justiceUser.VhoWorkHours.Where(x => !x.Deleted).ToList();

        await _context.SaveChangesAsync();

        var csos = new List<Guid>() { justiceUser.Id };
        
        // Act
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(includeWorkHours: true, cso: csos));
        
        // Assert
        hearings.Count.Should().Be(1);
        hearings[0].AllocatedTo.Id.Should().Be(justiceUser.Id);
        var resultingWorkHours = hearings[0].Allocations[0].JusticeUser.VhoWorkHours;
        hearings[0].Allocations[0].JusticeUser.VhoWorkHours.Count.Should().Be(nonDeletedWorkHours.Count);
        CollectionAssert.AreEquivalent(resultingWorkHours.Select(wh => wh.Id), nonDeletedWorkHours.Select(wh => wh.Id));
    }
    [Test]
    public async Task should_not_return_excluded_venues()
    {
        _seededHearing4 = await Hooks.SeedVideoHearing(status: BookingStatus.Booked, configureOptions: options =>
        {
            options.HearingVenue = new HearingVenue(474, "Dolgellau");
        });
        _seededHearing5 = await Hooks.SeedVideoHearing(status: BookingStatus.Booked, configureOptions: options =>
        {
            options.HearingVenue = new HearingVenue(15, "Aberdeen Tribunal Hearing Centre");
        });
        //ARRANGE
        var justiceUser = await Hooks.SeedJusticeUser(userName: "testUser", null, null, isTeamLead:true);
        await Hooks.AddAllocation(_seededHearing5, justiceUser);
        await Hooks.AddAllocation(_seededHearing2, justiceUser);
        await Hooks.AddAllocation(_seededHearing4, justiceUser);
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(cso:new[] {justiceUser.Id}));
        //ASSERT
        hearings.Count.Should().Be(1);
        
    }   
}
