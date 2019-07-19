using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetPersonByUsernameQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonByUsernameQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonByUsernameQueryHandler(context);
        }

        [Test]
        public async Task should_return_null_when_no_person_found()
        {
            var query = new GetPersonByUsernameQuery("doesnt.existatall@email.com");
            var person = await _handler.Handle(query);
            person.Should().BeNull();
        }
        
        [Test]
        public async Task should_return_person_that_exists()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var existingPerson = seededHearing.GetParticipants().First().Person;
            var query = new GetPersonByUsernameQuery(existingPerson.Username);
            var person = await _handler.Handle(query);
            person.Should().NotBeNull();
            person.Should().BeEquivalentTo(existingPerson);
        }
    }
}