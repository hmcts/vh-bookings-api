using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

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
            _seededHearing = await Hooks.SeedVideoHearing(options => options.ScheduledDate = DateTime.Today);
            var query = new GetHearingsForTodayQuery();
            var venues = await _handler.Handle(query);
            venues.FirstOrDefault().Id.Should().Be(_seededHearing.Id);
        }    
        
        [Test]
        public async Task Should_return_list_of_hearing_for_today_by_venue()
        {
            _seededHearing = await Hooks.SeedVideoHearing(options => options.ScheduledDate = DateTime.Today);
            var query = new GetHearingsForTodayQuery(new string[] { _seededHearing.HearingVenueName });
            var venues = await _handler.Handle(query);
            venues.FirstOrDefault().Id.Should().Be(_seededHearing.Id);
        }
    }
}