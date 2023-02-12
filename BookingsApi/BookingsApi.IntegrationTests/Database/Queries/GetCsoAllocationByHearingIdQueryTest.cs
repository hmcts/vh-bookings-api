using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries;

public class GetCsoAllocationByHearingIdQueryTest : DatabaseTestsBase
{
    private GetCsoAllocationByHearingIdQueryHandler _handler;
    private VideoHearing _seededHearing1;
    private BookingsDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        _context = new BookingsDbContext(BookingsDbContextOptions);
        _handler = new GetCsoAllocationByHearingIdQueryHandler(_context);
        _seededHearing1 = await Hooks.SeedVideoHearing();
    }
    
    [Test]
    public async Task Should_get_allocated_CSO_for_hearing()
    {
        //Arrange
        var justiceUser = await Hooks.SeedJusticeUser(userName: "testUser", "test", "user", isTeamLead:true);
        await Hooks.AddAllocation(_seededHearing1, justiceUser);  
        //ACT
        var response = await _handler.Handle(new GetCsoAllocationByHearingIdQuery(_seededHearing1.Id));
        //ASSERT
        response.Username.Should().BeEquivalentTo(justiceUser.Username);
    }
       
    [Test]
    public async Task Should_get_return_null_if_hearing_unallocated()
    { 
        //ACT
        var response = await _handler.Handle(new GetCsoAllocationByHearingIdQuery(_seededHearing1.Id));
        //ASSERT
        response.Should().BeNull();
    }
}
