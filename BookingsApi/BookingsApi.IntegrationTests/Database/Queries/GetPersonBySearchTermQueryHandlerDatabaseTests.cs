using BookingsApi.DAL.Queries;
using Moq;
using BookingsApi.Common.Services;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetPersonBySearchTermQueryHandlerTests : DatabaseTestsBase
    {
        private GetPersonBySearchTermQueryHandler _handler;
        private Mock<IFeatureToggles> _featureToggles;


        [SetUp]
        public void Setup()
        {
            _featureToggles = new Mock<IFeatureToggles>();
            var context = new BookingsDbContext(BookingsDbContextOptions);
            
            _featureToggles.Setup(toggle => toggle.EJudFeature()).Returns(false);
            _handler = new GetPersonBySearchTermQueryHandler(context, _featureToggles.Object);
        }

        [Test]
        public async Task Should_find_contact_by_email_case_insensitive()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            
            var person = seededHearing.GetParticipants().First(x => x is Individual).Person;
            var contactEmail = person.ContactEmail;
            
            // Build a search term based on lower and upper case of the expected email
            var twoCharactersLowercase = contactEmail.Substring(0, 2).ToLower();
            var twoCharactersUppercase = contactEmail.Substring(2, 2).ToUpper(); 
            var searchTerm =  twoCharactersLowercase + twoCharactersUppercase;
            var matches  = await _handler.Handle(new GetPersonBySearchTermQuery(searchTerm));

            matches.Select(m => m.Id).Should().Contain(person.Id);
        }

        [Test]
        public async Task Should_find_contact_by_email_when_searched_from_middle_of_string()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var person = seededHearing.GetParticipants().First(x => x is Individual).Person;
            var contactEmail = person.ContactEmail;
            
            var searchTerm = contactEmail.Substring(2, 3);
            var matches  = await _handler.Handle(new GetPersonBySearchTermQuery(searchTerm));

            matches.Select(m => m.Id).Should().Contain(person.Id);
        }
    }
}