using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingsByVenuesQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingsForTodayByVenuesQueryHandler _handler;
        private VideoHearing _seededHearing;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsForTodayByVenuesQueryHandler(context);
        }
        
        
        [Test]
        public async Task Should_return_list_of_hearing_venues()
        {
            _seededHearing = await Hooks.SeedVideoHearing(options => options.ScheduledDate = DateTime.Today);
            var query = new GetHearingsForTodayByVenuesQuery(new string[] { _seededHearing.HearingVenueName });
            var venues = await _handler.Handle(query);
            venues.FirstOrDefault().Id.Should().Be(_seededHearing.Id);
        }
    }
}