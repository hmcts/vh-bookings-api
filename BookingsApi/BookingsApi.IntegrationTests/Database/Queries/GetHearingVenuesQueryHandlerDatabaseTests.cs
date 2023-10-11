using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingVenuesQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingVenuesQueryHandler _handler;
        private List<HearingVenue> _testVenues;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingVenuesQueryHandler(_context);
        }
        
        
        [Test]
        public async Task Should_return_list_of_hearing_venues()
        {
            var venues = await _handler.Handle(new());
            venues.Should().NotBeEmpty();
            venues.Should().Contain(x => x.Name == "Manchester County and Family Court");
        }
        
        [Test]
        public async Task Should_return_list_of_hearing_venues_excluding_expired_venues()
        {
            _testVenues = new List<HearingVenue>()
            {
                new(100001, "test-venue-1") { ExpirationDate = DateTime.UtcNow.Date }, // EXPIRED
                new(100002, "test-venue-2") { ExpirationDate = DateTime.UtcNow}, // EXPIRED
                new(100004, "test-venue-3") { ExpirationDate = DateTime.UtcNow.Date.AddDays(-1) }, // EXPIRED
                new(100003, "test-venue-4") { ExpirationDate = DateTime.UtcNow.Date.AddDays(1) } // NOT EXPIRED
            };
            foreach (var venue in _testVenues)
                _context.Venues.Add(venue);
            
            await _context.SaveChangesAsync();
            
            var venues = await _handler.Handle(new GetHearingVenuesQuery(true));
            
            venues.Should().NotBeEmpty();
            venues.Should().NotContain(v => _testVenues[0].Id == v.Id);
            venues.Should().NotContain(v => _testVenues[1].Id == v.Id);
            venues.Should().NotContain(v => _testVenues[2].Id == v.Id);
            venues.Should().Contain(v => _testVenues[3].Id == v.Id);
        }      
        
        [TearDown]
        public async Task VenueTearDown()
        {
            if (_testVenues != null)
            {
                foreach (var venue in _testVenues)
                    _context.Venues.Remove(venue);
                _testVenues = null;
                await _context.SaveChangesAsync();
            }
        }
    }
}