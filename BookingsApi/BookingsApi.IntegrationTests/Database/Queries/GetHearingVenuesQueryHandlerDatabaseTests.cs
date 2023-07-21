using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries.V1;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingVenuesQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingVenuesQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingVenuesQueryHandler(context);
        }
        
        
        [Test]
        public async Task Should_return_list_of_hearing_venues()
        {
            var venues = await _handler.Handle(new GetHearingVenuesQuery());
            venues.Should().NotBeEmpty();
            venues.Should().Contain(x => x.Name == "Manchester County and Family Court");
        }
    }
}