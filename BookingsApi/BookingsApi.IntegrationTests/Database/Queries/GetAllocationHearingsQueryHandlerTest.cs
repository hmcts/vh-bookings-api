using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries;

public class GetAllocationHearingsQueryHandlerTest : DatabaseTestsBase
{
    private GetAllocationHearingsQueryHandler _handler;
    private VideoHearing _seededHearing1;
    private VideoHearing _seededHearing2;
    private VideoHearing _seededHearing3;
    private BookingsDbContext _context;

    [SetUp]
    public async Task Setup()
    {
        _context = new BookingsDbContext(BookingsDbContextOptions);
        _handler = new GetAllocationHearingsQueryHandler(_context);
        _seededHearing1 = await Hooks.SeedVideoHearingV2();
        _seededHearing2 = await Hooks.SeedVideoHearingV2();
        _seededHearing3 = await Hooks.SeedVideoHearingV2();
    }
    
    [Test]
    public async Task Should_get_hearings()
    {
        //ARRANGE
        var expectedResponses = new List<VideoHearing>()
        {
            _seededHearing1,
            _seededHearing2,
            _seededHearing3
        };
        //ACT
        var response = await _handler.Handle(new GetAllocationHearingsQuery(new[] {_seededHearing1.Id, _seededHearing2.Id, _seededHearing3.Id}));
        //ASSERT
        foreach (var expectedResponse in expectedResponses)
            response.Should().Contain(e => e.Id == expectedResponse.Id);
    }
}
