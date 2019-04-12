using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetPersonBySearchTermQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonBySearchTermQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonBySearchTermQueryHandler(context);
        }

        [Test]
        public async Task should_find_contact_by_email_case_insensitive()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var person = seededHearing.GetPersons().First();
            var contactEmail = person.ContactEmail;
            
            // Get a part of the email and change case
            var searchTerm = contactEmail.Substring(0, 2).ToLower() + contactEmail.Substring(2, 2);
            var matches  = await _handler.Handle(new GetPersonBySearchTermQuery(searchTerm));

            matches.Select(m => m.Id).Should().Contain(person.Id);
        }
    }
}