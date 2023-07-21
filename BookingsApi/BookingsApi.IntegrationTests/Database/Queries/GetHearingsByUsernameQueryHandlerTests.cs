using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using FluentAssertions;
using NUnit.Framework;

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

            var username = hearing2.GetPersons().First().Username;
            
            var query = new GetHearingsByUsernameQuery(username);

            var result = await _handler.Handle(query);
            result.Any().Should().BeTrue();
            result.Any(x => x.Id == hearing2.Id).Should().BeTrue();
            result.Any(x => x.Id == hearing1.Id).Should().BeFalse();
            result.Any(x => x.Id == hearing3.Id).Should().BeFalse();
        }

        [Test]
        public async Task Should_return_hearings_with_linked_participants_for_username()
        {
            var seededHearing = await Hooks.SeedVideoHearingLinkedParticipants(null);
            var username = seededHearing.GetPersons().First().Username;
            var query = new GetHearingsByUsernameQuery(username);

            var hearings = await _handler.Handle(query);
            hearings.Any().Should().BeTrue();

            var participants = hearings[0].GetParticipants();
            participants.Any().Should().BeTrue();
            var individuals = participants.Where(x => x.GetType() == typeof(Individual))
                .ToList();
            individuals[0].LinkedParticipants.Should().NotBeNull();
            individuals[0].LinkedParticipants[0].Type.Should().Be(LinkedParticipantType.Interpreter);
            individuals[1].LinkedParticipants.Should().NotBeNull();
            individuals[1].LinkedParticipants[0].Type.Should().Be(LinkedParticipantType.Interpreter);
        }
    }
}