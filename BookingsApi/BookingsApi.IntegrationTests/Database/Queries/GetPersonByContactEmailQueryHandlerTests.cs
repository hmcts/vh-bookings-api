using System.Linq;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetPersonByContactEmailQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonByContactEmailQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonByContactEmailQueryHandler(context);
        }

        [Test]
        public async Task Should_return_null_when_no_person_found()
        {
            var query = new GetPersonByContactEmailQuery("doesnt.existatall@hmcts.net");
            var person = await _handler.Handle(query);
            person.Should().BeNull();
        }
        
        [Test]
        public async Task Should_return_person_that_exists()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var existingPerson = seededHearing.GetParticipants().First().Person;
            var query = new GetPersonByContactEmailQuery(existingPerson.ContactEmail);
            var person = await _handler.Handle(query);
            person.Should().NotBeNull();
            person.Should().BeEquivalentTo(existingPerson);
        }
    }
}