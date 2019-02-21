using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Faker;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetPersonByUsernameQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonByUsernameQueryHandler _handler;
        private Guid _newHearingId;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonByUsernameQueryHandler(context);
            _newHearingId = Guid.Empty;
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

        [Test]
        public async Task should_return_null_when_no_person_found()
        {
            var query = new GetPersonByUsernameQuery(Internet.Email());
            var person = await _handler.Handle(query);
            person.Should().BeNull();
        }
        
        [Test]
        public async Task should_return_person_that_exists()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _newHearingId = seededHearing.Id;

            var existingPerson = seededHearing.GetParticipants().First().Person;
            var query = new GetPersonByUsernameQuery(existingPerson.Username);
            var person = await _handler.Handle(query);
            person.Should().NotBeNull();
            person.Should().BeEquivalentTo(existingPerson);
        }
    }
}