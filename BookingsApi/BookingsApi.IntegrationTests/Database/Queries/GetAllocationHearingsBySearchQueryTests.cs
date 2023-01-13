using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;

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
        var justiceUser = "testUser";
        await Hooks.AddAllocation(_seededHearing3, justiceUser);
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(csoUserName:new[] {justiceUser}));
        //ASSERT
        hearings.Count.Should().Be(1);
        hearings.First().HearingCases.First().Case.Number.Should().Be(_seededHearing3.HearingCases.First().Case.Number);
        hearings.First().AllocatedTo.Username.Should().Be(justiceUser);
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
        await Hooks.AddAllocation(_seededHearing2, "testUser");
        
        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(isUnallocated:true));

        //ASSERT
        hearings.Count.Should().Be(2);
        hearings[0].HearingCases.First().Case.Number.Should().Be(_seededHearing1.HearingCases.First().Case.Number);
        hearings[1].HearingCases.First().Case.Number.Should().Be(_seededHearing3.HearingCases.First().Case.Number);
    }
    
    [Test]
    public async Task Should_get_hearing_details_by_multiple_parameters()
    {
        //ARRANGE
        // hearing 1,2 & 3
        var justiceUser = "testUser";
        await Hooks.AddAllocation(_seededHearing1, justiceUser);
        await Hooks.AddAllocation(_seededHearing2, justiceUser);
        await Hooks.AddAllocation(_seededHearing3, justiceUser);
        
        //hearing 1 & 2
        var caseType = TestCaseType;
        
        // hearing 1 & 3
        var fromDate = DateTime.Today.AddDays(48); //before _testDate1
        var toDate = DateTime.Today.AddDays(52);  //after _testDate1

        //ACT
        var hearings = await _handler.Handle(new GetAllocationHearingsBySearchQuery(fromDate:fromDate, toDate:toDate, caseType:new[]{caseType}, csoUserName:new[]{justiceUser}));

        //ASSERT
        hearings.Count.Should().Be(1);
        hearings.First().HearingCases.First().Case.Number.Should().Be(_seededHearing1.HearingCases.First().Case.Number);
        hearings.First().ScheduledDateTime.Should().Be(_seededHearing1.ScheduledDateTime);
        hearings.First().CaseType.Name.Should().Be(_seededHearing1.CaseType.Name);
        hearings.First().AllocatedTo.Username.Should().Be(justiceUser);
    }
}
