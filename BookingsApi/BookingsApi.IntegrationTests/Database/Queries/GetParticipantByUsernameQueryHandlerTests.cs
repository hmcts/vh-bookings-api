using System.Linq;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetParticipantByUsernameQueryHandlerTests : DatabaseTestsBase
    {
        private GetParticipantsByUsernameQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetParticipantsByUsernameQueryHandler(context);
        }

        [Test]
        public async Task Should_return_empty_when_no_participant_found()
        {
            var query = new GetParticipantsByUsernameQuery("doesnt.existatall@hmcts.net");
            var participants = await _handler.Handle(query);

            participants.Should().BeEmpty();
        }
        
        [Test]
        public async Task Should_return_participant_that_exists()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var firstParticipant = seededHearing.GetParticipants().First();
            var query = new GetParticipantsByUsernameQuery(firstParticipant.Person.Username);
            var participant = (await _handler.Handle(query)).FirstOrDefault();

            participant.Should().NotBeNull();
            participant.PersonId.Should().Be(firstParticipant.PersonId);
            participant.Person.Username.Should().Be(firstParticipant.Person.Username);
            participant.Id.Should().Be(firstParticipant.Id);
        }
    }
}