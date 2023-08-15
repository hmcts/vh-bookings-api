using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingsForTodayQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingsForTodayQueryHandler _handler;
        private VideoHearing _seededHearing;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsForTodayQueryHandler(context);
        }
            
        
        [Test]
        public async Task Should_return_list_of_hearing_for_today()
        {
            _seededHearing = await Hooks.SeedVideoHearing(options => options.ScheduledDate = DateTime.UtcNow.Date);
            var query = new GetHearingsForTodayQuery();
            var venues = await _handler.Handle(query);
            venues.Should().Contain(x=> x.Id == _seededHearing.Id);
        }    
        
        [Test]
        public async Task Should_return_list_of_hearing_for_today_by_venue()
        {
            _seededHearing = await Hooks.SeedVideoHearing(options => options.ScheduledDate = DateTime.UtcNow.Date);
            var query = new GetHearingsForTodayQuery(new string[] { _seededHearing.HearingVenueName });
            var venues = await _handler.Handle(query);
            venues.Should().Contain(x=> x.Id == _seededHearing.Id);
        }
    }
}