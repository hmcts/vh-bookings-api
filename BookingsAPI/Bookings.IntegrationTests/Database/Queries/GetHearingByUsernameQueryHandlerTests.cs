using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetHearingByUsernameQueryHandlerTests : DatabaseTestsBase
    {
        private GetHearingForUsernameQueryHandler _handler;
        private Guid _newHearingId;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingForUsernameQueryHandler(context);
            _newHearingId = Guid.Empty;
        }
        
        [Test]
        public async Task should_return_empty_list()
        {
            var username = "nobody@test.com";
            var query = new GetHearingsForUsernameQuery(username);
            var hearings = await _handler.Handle(query);

            hearings.Should().BeEmpty();
        }
        
        [Test]
        public async Task should_hearing_for_user()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;
            
            var username = seededHearing.GetParticipants().First().Person.Username;
            var query = new GetHearingsForUsernameQuery(username);
            var hearings = await _handler.Handle(query);
            
            hearings.Should().NotBeEmpty();
            hearings.Count.Should().Be(1);
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newHearingId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {_newHearingId}");
                await Hooks.RemoveVideoHearing(_newHearingId);
            }
        }
    }
}