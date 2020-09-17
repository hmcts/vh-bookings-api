using System;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetPersonByOldHearings : DatabaseTestsBase
    {
        private GetPersonsByClosedHearingsQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonsByClosedHearingsQueryHandler(context);
        }

        [Test]
        public async Task Should_return_list_of_user_names_for_old_hearings_over_3_months_old()
        {
            var oldHearings = await Hooks.SeedPastHearings(DateTime.UtcNow.AddMonths(-4));
            TestContext.WriteLine($"New seeded video hearing id: { oldHearings.Id }");

            var usernamesList = await _handler.Handle(new GetPersonsByClosedHearingsQuery());
            usernamesList.Should().NotBeNull();
            usernamesList.Count.Should().Be(3); // Individual & Representative participants
        }
    }
}
