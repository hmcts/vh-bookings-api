using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetPersonBySearchTermQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonBySearchTermQueryHandler _handler;
        private Mock<IFeatureFlagService> _featureFlagService;

        [SetUp]
        public void Setup()
        {
            _featureFlagService = new Mock<IFeatureFlagService>();
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonBySearchTermQueryHandler(context, _featureFlagService.Object);
        }

        [Test]
        public async Task Should_find_contact_by_email_case_insensitive()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var person = seededHearing.GetPersons().First(x => seededHearing.Participants.Any(p => p.PersonId == x.Id && !GetPersonBySearchTermQueryHandler.excludedRoles.Contains(p.Discriminator)));
            var contactEmail = person.ContactEmail;
            
            // Build a search term based on lower and upper case of the expected email
            var twoCharactersLowercase = contactEmail.Substring(0, 2).ToLower();
            var twoCharactersUppercase = contactEmail.Substring(2, 2).ToUpper(); 
            var searchTerm =  twoCharactersLowercase + twoCharactersUppercase;
            var matches  = await _handler.Handle(new GetPersonBySearchTermQuery(searchTerm));

            matches.Select(m => m.Id).Should().Contain(person.Id);
        }
    }
}