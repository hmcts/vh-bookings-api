using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingsByUsernameQueryHandlerTests : DatabaseTestsBase
    {
        private GetHearingsByUsernameQueryHandler _handler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsByUsernameQueryHandler(context);
        }

        [Test]
        public async Task Should_return_hearings_for_username()
        {
            var hearing1 = await Hooks.SeedVideoHearing();
            var hearing2 = await Hooks.SeedVideoHearing();
            var hearing3 = await Hooks.SeedVideoHearing();

            var username = hearing2.GetPersons()[0].Username;
            
            var query = new GetHearingsByUsernameQuery(username);

            var result = await _handler.Handle(query);
            result.Any().Should().BeTrue();
            result.Exists(x => x.Id == hearing2.Id).Should().BeTrue();
            result.Exists(x => x.Id == hearing1.Id).Should().BeFalse();
            result.Exists(x => x.Id == hearing3.Id).Should().BeFalse();
        }

        [Test]
        public async Task Should_return_hearings_with_linked_participants_for_username()
        {
            var seededHearing = await Hooks.SeedVideoHearing(withLinkedParticipants: true);
            var username = seededHearing.GetParticipants().First(p => p is Individual && p.LinkedParticipants.Any()).Person.Username;
            var query = new GetHearingsByUsernameQuery(username);

            var hearings = await _handler.Handle(query);
            hearings.Any().Should().BeTrue();

            var participants = hearings[0].GetParticipants();
            participants.Any().Should().BeTrue();
            var individual = participants.First(x => x.Person.Username == username);
            individual.LinkedParticipants[0].Type.Should().Be(LinkedParticipantType.Interpreter);

            participants.First(x => x.Id == individual.LinkedParticipants[0].LinkedId).LinkedParticipants.Should()
                .Contain(x => x.LinkedId == individual.Id);
        }
    }
}